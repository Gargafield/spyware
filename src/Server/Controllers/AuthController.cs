using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using Server.Services;
using Server.Utils;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    

    [HttpPost("login", Name = "Login")]
    [AllowAnonymous]
    public IActionResult Login(LoginModel loginModel) {

        // Check if user exists
        // TODO: Implement Lectio login
        bool auth = loginModel.Username == "admin" && loginModel.Password == "admin";

        if (auth) {
            return Ok(new { AccessToken = SetCookie(loginModel.Username) });
        }

        return Unauthorized();
    }

    [HttpPost("refresh", Name = "Refresh")]
    [AllowAnonymous]
    public IActionResult Refresh(RefreshModel refreshModel) {
        if (_authService.ValidateToken(refreshModel.AccessToken, out var username)) {
            return Ok(new { AccessToken = SetCookie(username) });
        }

        return Unauthorized();
    }

    private string SetCookie(string username) {

        var token = _authService.CreateToken(username);

        Response.Cookies.Append("access_token", token, new CookieOptions {
            SameSite = SameSiteMode.Strict,
            Secure = true
        });

        return token;
    }
}
