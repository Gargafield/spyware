
using System.Security.Claims;
using System.Text.Json;
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
        
        if (await _localStorage.ContainsKey("token")
            && await _localStorage.ContainsKey("user")) {
            try {
                var response = await _httpService.GetAsync<LoginResultModel>("api/auth/login");
                return response!.Succeeded;
            }
            catch (Exception) {
                await _localStorage.Clear();
            }
        }

        return false;
    }


    private AuthenticationState _anonymous() => new(new ClaimsPrincipal(new ClaimsIdentity()));
    private AuthenticationState _authenticated(User user) {
        if (user == null)
            return _anonymous();

        return new(new ClaimsPrincipal(new ClaimsIdentity([
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)]
        , "jwt")));
    } 

    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        if (await IsLoggedIn()) {
            var user = await _localStorage.GetItem<User>("user");
            return _authenticated(user);
        }

        return _anonymous();
    }

    public void LoggedIn(User user) {
        NotifyAuthenticationStateChanged(Task.FromResult(_authenticated(user)));
    }

    public void LoggedOut() {
        Console.WriteLine("User logged out");
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous()));
    }
}
