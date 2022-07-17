using DatabaseModels.DataTransferObjets;

namespace DatabaseModels.InitialObjects;

public class ScoreboardRecord
{
    public int Id { get; set; }
    
    public int Score { get; set; }
    public DateTime PostTime { get; set; }
    
    public User User { get; set; } = null!;

    public ScoreboardRecordDto ToDto() => new() { Id = Id, Score = Score, PostTime = PostTime, User = User.ToDto() };
}