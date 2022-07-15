using System.ComponentModel.DataAnnotations;

namespace DatabaseModels;

public class ScoreboardRecord
{
    [Key]
    public string? Username { get; set; }
    public int Score { get; set; }
    
    public DateTime PostTime { get; set; }
}