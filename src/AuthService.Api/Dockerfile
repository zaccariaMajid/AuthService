# ============================
#  STAGE 1: BUILD
# ============================
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ARG TARGETARCH

# Copy csproj first to leverage layer caching
COPY src/AuthService.Domain/AuthService.Domain.csproj src/AuthService.Domain/
COPY src/AuthService.Application/AuthService.Application.csproj src/AuthService.Application/
COPY src/AuthService.Infrastructure/AuthService.Infrastructure.csproj src/AuthService.Infrastructure/
COPY src/AuthService.Api/AuthService.Api.csproj src/AuthService.Api/

RUN dotnet restore src/AuthService.Api/AuthService.Api.csproj

# Copy the rest of the source
COPY src/ src/

# Publish for the target arch (e.g., linux-arm64)
RUN dotnet publish src/AuthService.Api/AuthService.Api.csproj -c Release -o /app/publish -r linux-$TARGETARCH --self-contained false /p:PublishSingleFile=true

# ============================
#  STAGE 2: RUNTIME
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AuthService.Api.dll"]
