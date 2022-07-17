namespace DatabaseModels.DataTransferObjets;

public class ScoreboardRecordDto
{
    public int Id { get; set; }
    
    public int Score { get; set; }
    public DateTime PostTime { get; set; }
    
    public UserDto User { get; set; } = null!;
}