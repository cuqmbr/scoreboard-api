using DatabaseModels.DataTransferObjets;

namespace DatabaseModels.InitialObjects;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;

    public string Role { get; set; } = null!;
    
    public ICollection<ScoreboardRecord> ScoreboardRecords { get; set; } = null!;

    public UserDto ToDto() => new() { Id = Id, Username = Username };
}