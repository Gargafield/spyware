
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models;
using Web.Services;

namespace Web;

public class AuthProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly IHttpService _httpService;
    private readonly ILogger<AuthProvider> _logger;

    public AuthProvider(ILocalStorageService localStorage, IHttpService httpService, ILogger<AuthProvider> logger) {
        _localStorage = localStorage;
        _httpService = httpService;
        _logger = logger;
    }

    private async Task<bool> IsLoggedIn() {
        
        if (await _localStorage.ContainsKey("token")) {
            try {
                var response = await _httpService.GetAsync<LoginResultModel>("api/auth/login");
                return response!.Succeeded;
            }
            catch (Exception) {
                await _localStorage.RemoveItem("token");
            }
        }

        return false;
    }


    private AuthenticationState _anonymous() => new(new ClaimsPrincipal(new ClaimsIdentity()));
    private AuthenticationState _authenticated(string token) => new(new ClaimsPrincipal(new ClaimsIdentity([
        new Claim(ClaimTypes.Name, token),
        new Claim(ClaimTypes.Role, "user")]
        , "jwt")));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        if (await IsLoggedIn()) {
            Console.WriteLine("User is logged in");
            var token = await _localStorage.GetItem<string>("token");
            return _authenticated(token);
        }

        Console.WriteLine("User is not logged in");
        return _anonymous();
    }

    public void LoggedIn(string token) {
        Console.WriteLine("User logged in");
        NotifyAuthenticationStateChanged(Task.FromResult(_authenticated(token)));
    }

    public void LoggedOut() {
        Console.WriteLine("User logged out");
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous()));
    }
}
