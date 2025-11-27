using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure;

public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var apiProjectPath = FindApiProjectPath(Directory.GetCurrentDirectory());

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("ConnectionString")
            ?? configuration["ConnectionString"]
            ?? throw new InvalidOperationException("Connection string 'ConnectionString' not found for design-time DbContext creation.");

        var builder = new DbContextOptionsBuilder<AuthDbContext>();
        builder.UseNpgsql(connectionString);

        return new AuthDbContext(builder.Options);
    }

    private static string FindApiProjectPath(string basePath)
    {
        // When the EF CLI runs from the API project, appsettings.* already exist in the current directory.
        if (File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            return basePath;
        }

        var fromRoot = Path.Combine(basePath, "src", "AuthService.Api");
        if (File.Exists(Path.Combine(fromRoot, "appsettings.json")))
        {
            return fromRoot;
        }

        var sibling = Path.GetFullPath(Path.Combine(basePath, "..", "AuthService.Api"));
        if (File.Exists(Path.Combine(sibling, "appsettings.json")))
        {
            return sibling;
        }

        throw new InvalidOperationException("Could not locate appsettings.json for design-time DbContext creation.");
    }
}
