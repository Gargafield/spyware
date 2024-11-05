using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Server.Services;
using Shared.Models;

namespace Server.Auth;

public class AuthHandler : AuthenticationHandler<AuthSchemeOptions> {
    private IAuthService _authService;
    private IUserService _userService;

    public AuthHandler(IOptionsMonitor<AuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IAuthService authService, IUserService userService) : base(options, logger, encoder) {
        _authService = authService;
        _userService = userService;
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

    private AuthenticateResult _success(string username, string role, int id)
        => AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        }, Scheme.Name)), Scheme.Name));

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? token = GetToken(Request);
        if (string.IsNullOrWhiteSpace(token))
            return _fail("Authorization header is empty");

        if (_authService.ValidateToken(token, out var username)
        && _userService.TryGetUser(username, out var user)) {
            if (user is Teacher)
                return _success(username, "Teacher", user.Id);

            if (user is Student)
                return _success(username, "Student", user.Id);

            return _fail("Invalid user type");
        }

        return _fail("Invalid token");
    }
}