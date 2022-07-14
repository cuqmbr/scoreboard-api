namespace DatabaseModels;

public class ScoreboardRecord
{
    public UInt32 Id { get; set; }

    public string? Username { get; set; }
    public DateTime PostTime { get; set; }
    public int Score { get; set; }
}