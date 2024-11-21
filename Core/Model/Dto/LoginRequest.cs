namespace Core.Model.Dto;

public record LoginRequest(string? Username, string? Password)
{
    public string? Username { get; } = Username;
    public string? Password { get; } = Password;
}