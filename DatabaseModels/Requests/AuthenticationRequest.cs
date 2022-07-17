namespace DatabaseModels.Requests;

public class AuthenticationRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}