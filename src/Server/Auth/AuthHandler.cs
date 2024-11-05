
using System.Net.Http.Headers;
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

    public static string? GetToken(HttpRequest request) {
        var authHeader = request.Headers.Authorization.FirstOrDefault();
        if (authHeader != null) {
            var scheme = AuthenticationHeaderValue.Parse(authHeader);
            return scheme.Parameter;
        }

        return request.Cookies["access_token"];
    }

    private AuthenticateResult _fail(string message)
        => AuthenticateResult.Fail(message);

    private AuthenticateResult _success(string username)
        => AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "user")
        }, Scheme.Name)), Scheme.Name));

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? token = GetToken(Request);
        if (string.IsNullOrWhiteSpace(token))
            return _fail("Authorization header is empty");

        if (_authService.ValidateToken(token, out var username))
            return _success(username);

        return _fail("Invalid token");
    }
}