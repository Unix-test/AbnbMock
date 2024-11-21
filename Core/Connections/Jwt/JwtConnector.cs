namespace Core.Connections.Jwt;

public class JwtConnector : IJwtConnector
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string TokenValidityInMinutes { get; set; }
    public required string RefreshTokenValidityInDays { get; set; }
}