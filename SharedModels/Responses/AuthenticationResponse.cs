namespace DatabaseModels.Responses;

public class AuthenticationResponse
{
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; } = null!;
    
    public string Token { get; set; } = null!;
}