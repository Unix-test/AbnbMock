using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Connections.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Helpers;

public class Jwt<T>(UserManager<T> userManager, IConfiguration configuration)
    where T : IdentityUser<Guid>
{
    public async Task<JwtSecurityToken> TokenGenerate(T user, bool? isRemember = false)
    {
        if (userManager is null) throw new ArgumentException("Cannot create userManager instance");

        var jwtConnector = configuration.GetSection(nameof(JwtConnector)).Get<JwtConnector>();

        var userRoles = await userManager.GetRolesAsync(user);

        var userInfos = new { user.Id };

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, userInfos.Id.ToString())
        };
        
        if(isRemember is true) authClaims.Add(new Claim(ClaimTypes.IsPersistent, "true"));

        authClaims.Add(new Claim("act", user.EmailConfirmed.ToString()));

        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var authSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtConnector?.SecretKey ?? string.Empty)
        );

        _ = int.TryParse(
            configuration[jwtConnector?.TokenValidityInMinutes ?? string.Empty],
            out var tokenValidityInMinutes
        );

        var token = new JwtSecurityToken(
            jwtConnector?.Issuer,
            jwtConnector?.Audience,
            expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
            claims: authClaims,
            signingCredentials: new SigningCredentials(
                authSigningKey,
                SecurityAlgorithms.HmacSha512
            )
        );

        return token;
    }

    public ClaimsPrincipal GetPrincipalFromToken(string? header, bool isRefresh)
    {
        var token = header is not null && header.Contains(value: "Bearer")
            ? header.Trim()
                .Replace(oldValue: "Bearer", newValue: string.Empty, comparisonType: StringComparison.OrdinalIgnoreCase)
                .Trim()
            : header;

        var jwtConnector = configuration.GetSection(key: nameof(JwtConnector)).Get<JwtConnector>();

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                key: Encoding.UTF8.GetBytes(s: jwtConnector?.SecretKey ?? string.Empty)
            ),
            ValidateLifetime = false,
            ValidIssuer = jwtConnector?.Issuer,
            ValidAudience = jwtConnector?.Audience,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true
        };

        if (string.IsNullOrEmpty(value: token))
            throw new UnauthorizedAccessException();

        var tokenHandler = new JwtSecurityTokenHandler();

        if (!CheckTokenIsExpired(handler: tokenHandler, token: token, isRefresh: isRefresh)) throw new ArgumentException("Token is not expired");

        var principal = tokenHandler.ValidateToken(
            token: token,
            validationParameters: tokenValidationParameters,
            validatedToken: out var securityToken
        );

        if (
            securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(
                value: SecurityAlgorithms.HmacSha512,
                comparisonType: StringComparison.InvariantCultureIgnoreCase
            )
        )
            throw new ArgumentException("Ivalid token");

        return principal;
    }

    private static bool CheckTokenIsExpired(JwtSecurityTokenHandler? handler, string? token, bool isRefresh = false)
    {
        if (isRefresh) return true;

        var jwtSecurityToken = handler?.ReadJwtToken(token);
        var tokenExp = jwtSecurityToken?.Claims.FirstOrDefault(claim =>
            claim.Type.Equals("exp")
        )?.Value;
        if (tokenExp is null) return false;
        var ticks = long.Parse(tokenExp);
        var tokenDate = DateTimeOffset.FromUnixTimeMilliseconds(ticks);
        var now = DateTime.Now.ToUniversalTime();
        var valid = tokenDate <= now;
        return valid;
    }
}