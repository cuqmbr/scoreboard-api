using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username))
        {
            return (false, "Username is taken.");
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

    public async Task<int> GetIdByUsername(string username)
    {
        var dbUser = await _dbContext.Users.FirstAsync(u => u.Username == username);
        return dbUser.Id;
    }
    
    private ClaimsIdentity AssembleClaimsIdentity(User user)
    {
        var subject = new ClaimsIdentity(new[] {
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
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
}