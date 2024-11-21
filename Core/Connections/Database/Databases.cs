namespace Core.Connections.Database;

public class Database : IDatabases
{
    public string Host { get; set; } = string.Empty;
    public string DataBase { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}