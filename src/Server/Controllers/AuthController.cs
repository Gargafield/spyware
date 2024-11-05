using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Auth;
using Server.Services;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private IAuthService _authService;
    private IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService) {
        _authService = authService;
        _userService = userService;
    }

    [HttpGet("login", Name = "Check")]
    [Authorize]
    public IActionResult Check() {
        var token = AuthHandler.GetToken(Request);
        return Ok(new LoginResultModel() {
            Succeeded = true,
            Token = token!
        });
    }

    [HttpPost("login", Name = "Login")]
    [AllowAnonymous]
    public IActionResult Login(LoginModel loginModel) {

        // Check if user exists
        // TODO: Implement Lectio login
        bool auth = loginModel.Username == loginModel.Password
            && _userService.DoesUserExist(loginModel.Username);

        if (auth) {
            return Ok(new LoginResultModel() {
                Succeeded = true,
                Token = SetCookie(loginModel.Username)
            });
        }

        return Unauthorized(new LoginResultModel() {
            Succeeded = false,
            Message = "Invalid username or password"
        });
    }

    [HttpPut("login", Name = "Refresh")]
    [AllowAnonymous]
    public IActionResult Refresh(RefreshModel refreshModel) {
        if (_authService.ValidateToken(refreshModel.AccessToken, out var username)) {
            return Ok(new { AccessToken = SetCookie(username) });
        }

        return Unauthorized();
    }

    [HttpDelete("login", Name = "Logout")]
    [Authorize]
    public IActionResult Logout() {
        var token = AuthHandler.GetToken(Request);
        if (token != null && _authService.RemoveToken(token)) {
            Response.Cookies.Delete("access_token");
            return Ok();
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
