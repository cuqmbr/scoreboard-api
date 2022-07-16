namespace DatabaseModels.Plain;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    
    public ICollection<ScoreboardRecord> ScoreboardRecords { get; set; } = null!;
}