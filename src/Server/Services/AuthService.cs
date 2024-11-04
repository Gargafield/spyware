
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Server.Utils;

namespace Server.Services;

public interface IAuthService {
    public bool ValidateToken(string token, out string username);
    public string CreateToken(string username);
    public bool RemoveToken(string token);
}

public class AuthService : IAuthService {
    private static EphemeralKeyStore<string, string> _tokenStore = new();
    private static readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(15);
    private IConfiguration _config;

    public AuthService(IConfiguration config) {
        _config = config;
    }

    public bool ValidateToken(string token, out string username) {
        return _tokenStore.TryGetValue(token, out username);
    }

    public bool RemoveToken(string token) {
        return _tokenStore.Remove(token);
    }

    public string CreateToken(string username) {
        var claims = new[] {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.Add(_expirationTime),
            signingCredentials: creds
        );
        var str = new JwtSecurityTokenHandler().WriteToken(token);

        _tokenStore.Add(str, username, _expirationTime);

        return str;
    }
}