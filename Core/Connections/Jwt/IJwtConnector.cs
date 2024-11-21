namespace Core.Connections.Jwt;

public interface IJwtConnector
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string TokenValidityInMinutes { get; set; }
    public string RefreshTokenValidityInDays { get; set; }
}