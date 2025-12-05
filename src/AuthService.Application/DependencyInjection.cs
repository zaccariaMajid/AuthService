using AuthService.Application.Permissions.CreatePermission;
using AuthService.Application.Roles.AddPermissionToRole;
using AuthService.Application.Roles.CreateRole;
using AuthService.Application.Products.CreateProduct;
using AuthService.Application.Tenants.AssignProductToTenant;
using AuthService.Application.Tenants.ActivateTenant;
using AuthService.Application.Tenants.CreateTenant;
using AuthService.Application.Tenants.DeactivateTenant;
using AuthService.Application.Users.ActivateUser;
using AuthService.Application.Users.AddRoleToUser;
using AuthService.Application.Users.ChangePassword;
using AuthService.Application.Users.DeactivateUser;
using AuthService.Application.Users.RefreshToken;
using AuthService.Application.Users.RegisterUser;
using AuthService.Application.Users.UserLogin;
using AuthService.Domain.Common.Results;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse>>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<UserLoginCommand, Result<UserLoginResponse>>, UserLoginCommandHandler>();
        services.AddScoped<ICommandHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>, RefreshTokenCommandHandler>();
        services.AddScoped<ICommandHandler<ChangePasswordCommand, Result>, ChangePasswordCommandHandler>();
        services.AddScoped<ICommandHandler<ActivateUserCommand, Result>, ActivateUserCommandHandler>();
        services.AddScoped<ICommandHandler<DeactivateUserCommand, Result>, DeactivateUserCommandHandler>();
        services.AddScoped<ICommandHandler<AddRoleToUserCommand, Result>, AddRoleToUserCommandHandler>();
        services.AddScoped<ICommandHandler<CreateRoleCommand, Result<CreateRoleResponse>>, CreateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<AddPermissionToRoleCommand, Result>, AddPermissionToRoleCommandHandler>();
        services.AddScoped<ICommandHandler<CreatePermissionCommand, Result<CreatePermissionResponse>>, CreatePermissionCommandHandler>();
        services.AddScoped<ICommandHandler<CreateTenantCommand, Result<CreateTenantResponse>>, CreateTenantCommandHandler>();
        services.AddScoped<ICommandHandler<ActivateTenantCommand, Result>, ActivateTenantCommandHandler>();
        services.AddScoped<ICommandHandler<DeactivateTenantCommand, Result>, DeactivateTenantCommandHandler>();
        services.AddScoped<ICommandHandler<CreateProductCommand, Result<CreateProductResponse>>, CreateProductCommandHandler>();
        services.AddScoped<ICommandHandler<AssignProductToTenantCommand, Result>, AssignProductToTenantCommandHandler>();

        return services;
    }
}
