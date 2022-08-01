using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DatabaseModels.InitialObjects;
using DatabaseModels.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;

namespace Server.Services;

public class AuthenticationService
{
    private readonly ServerDbContext _dbContext;
    private readonly Jwt _jwt;

    public AuthenticationService(ServerDbContext dbContext, Jwt jwt)
    {
        _dbContext = dbContext;
        _jwt = jwt;
    }

    public async Task<(bool success, string content)> Register(AuthenticationRequest request)
    {
        if (!IsValidUsername(request.Username, out string usrErr))
        {
            return (false, usrErr);
        }
        
        if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username))
        {
            return (false, "Username is taken.");
        }

        if (!IsValidPassword(request.Password, out string pwdErr))
        {
            return (false, pwdErr);
        }

        var user = new User {
            Username = request.Username,
            PasswordHash = request.Password,
            Role = "Default"
        };
        ProvideSaltAndHash(user);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return (true, "");
    }

    public async Task<(bool success, string content)> Login(AuthenticationRequest request)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            return (false, "Invalid username.");
        }

        if (user.PasswordHash != ComputeHash(request.Password, user.PasswordSalt))
        {
            return (false, "Invalid password.");
        }

        return (true, GenerateJwtToken(AssembleClaimsIdentity(user)));
    }

    private ClaimsIdentity AssembleClaimsIdentity(User user)
    {
        var subject = new ClaimsIdentity(new[] {
            new Claim("id", user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        });

        return subject;
    }
    
    private string GenerateJwtToken(ClaimsIdentity subject)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwt.Key);
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = subject,
            Audience = _jwt.Audience,
            Issuer = _jwt.Issuer,
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private void ProvideSaltAndHash(User user)
    {
        var salt = GenerateSalt();
        user.PasswordSalt = Convert.ToBase64String(salt);
        user.PasswordHash = ComputeHash(user.PasswordHash, user.PasswordSalt);
    }

    private byte[] GenerateSalt()
    {
        var rng = RandomNumberGenerator.Create();
        var salt = new byte[24];
        rng.GetBytes(salt);
        return salt;
    }

    private string ComputeHash(string password, string saltString)
    {
        var salt = Convert.FromBase64String(saltString);

        using var hashGenerator = new Rfc2898DeriveBytes(password, salt);
        hashGenerator.IterationCount = 10101;
        var bytes = hashGenerator.GetBytes(24);
        return Convert.ToBase64String(bytes);
    }

    private bool IsValidUsername(string username, out string validationError)
    {
        if (username.Contains(" "))
        {
            validationError = "Username must be a one word.";
            return false;
        }

        if (username.Length < 3)
        {
            validationError = "Username must be minimum 3 characters long.";
            return false;
        }

        if (username.Length > 16)
        {
            validationError = "Username must be maximum 16 characters long.";
        }
        
        validationError = String.Empty;
        return true;
    }
    
    private bool IsValidPassword(string password, out string validationError)
    {
        string defaultValidationError = "Invalid password.";
        
        if (String.IsNullOrEmpty(password) || String.IsNullOrWhiteSpace(password))
        {
            validationError = defaultValidationError;
            return false;
        }

        if (password.Length < 8)
        {
            validationError = "Password must be minimum 8 characters long.";
            return false;
        }
        
        if (password.Length > 32)
        {
            validationError = "Password must be maximum 32 characters long.";
            return false;
        }

        var regEx = new Regex("^(?=.*[a-z])(?=.*[A-Z]).{8,}$");

        if (!regEx.IsMatch(password))
        {
            validationError = "Password must contain at least 1 upper, 1 lower case letters and 1 number.";
            return false;
        }

        validationError = String.Empty;
        return true;
    }
}