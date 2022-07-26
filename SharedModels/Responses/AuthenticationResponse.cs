namespace DatabaseModels.Responses;

public class AuthenticationResponse
{
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
}