namespace Core.Connections.Database;

public interface IDatabases
{
    public string Host { get; set; }
    public string DataBase { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}