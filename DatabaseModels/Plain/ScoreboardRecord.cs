namespace DatabaseModels.Plain;

public class ScoreboardRecord
{
    public int Id { get; set; }
    
    public int Score { get; set; }
    public DateTime PostTime { get; set; }
    
    public User User { get; set; }
}