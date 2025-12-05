using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SymmetricSecurityKey _securityKey;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value ?? throw new ArgumentNullException(nameof(options));

        ValidateSettings();

        _tokenHandler = new JwtSecurityTokenHandler();
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
    }

    public string GenerateAccessToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var claims = BuildClaims(user);
        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            NotBefore = now,
            IssuedAt = now,
            Expires = now.AddMinutes(_settings.AccessTokenMinutes),
            SigningCredentials = credentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(randomBytes);
        var expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);

        return RefreshToken.Create(userId, token, expires);
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            throw new ArgumentException("JWT SecretKey cannot be null or empty.", nameof(_settings.SecretKey));

        if (_settings.SecretKey.Length < 32)
            throw new ArgumentException("JWT SecretKey must be at least 32 characters long.", nameof(_settings.SecretKey));

        if (string.IsNullOrWhiteSpace(_settings.Issuer))
            throw new ArgumentException("JWT Issuer cannot be null or empty.", nameof(_settings.Issuer));

        if (string.IsNullOrWhiteSpace(_settings.Audience))
            throw new ArgumentException("JWT Audience cannot be null or empty.", nameof(_settings.Audience));
    }

    private static IEnumerable<Claim> BuildClaims(User user)
    {
        // JWT standard claims
        yield return new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString());
        yield return new Claim(JwtRegisteredClaimNames.Email, user.Email.Value);
        yield return new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());

        // .NET Identity claims
        yield return new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());
        yield return new Claim(ClaimTypes.Name, user.Email.Value);
        yield return new Claim(ClaimTypes.Email, user.Email.Value);
        yield return new Claim("tenant", user.TenantId.ToString());

        // Roles
        foreach (var role in user.Roles)
            yield return new Claim(ClaimTypes.Role, role.Name);
    }
}
