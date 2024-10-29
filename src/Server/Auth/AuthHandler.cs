
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Server.Controllers;
using Server.Services;

namespace Server.Auth;

public class AuthHandler : AuthenticationHandler<AuthSchemeOptions>
{
    private IAuthService _authService;

    public AuthHandler(IOptionsMonitor<AuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IAuthService authService) : base(options, logger, encoder) {
        _authService = authService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? token = (string?)Request.Headers["Authorization"] ?? Request.Cookies["access_token"];
        if (string.IsNullOrWhiteSpace(token)) {
            return AuthenticateResult.Fail("Authorization header is empty");
        }

        if (_authService.ValidateToken(token, out var username)) {
            var claims = new[] {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "admin")
            };
            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));
        }

        return AuthenticateResult.Fail("Invalid token");
    }
}