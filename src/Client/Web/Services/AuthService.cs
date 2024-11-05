using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models;

namespace Web.Services;

public interface IAuthService {
    Task<bool> IsAuthenticated();
    Task<LoginResultModel> LoginAsync(LoginModel model);
    Task LogoutAsync();
}

public class AuthService : IAuthService {
    private readonly ILocalStorageService _localStorageService;
    private readonly IHttpService _httpService;
    private readonly AuthenticationStateProvider _authProvider;

    public AuthService(ILocalStorageService localStorageService, IHttpService httpService, AuthenticationStateProvider authProvider) {
        _localStorageService = localStorageService;
        _httpService = httpService;
        _authProvider = authProvider;
    }

    public async Task<bool> IsAuthenticated() {
        var state = await _authProvider.GetAuthenticationStateAsync();
        var identity = state.User.Identity;
        return identity != null && identity.IsAuthenticated;
    }

    public async Task<LoginResultModel> LoginAsync(LoginModel model) {
        var response = await _httpService.PostAsync<LoginResultModel>("api/auth/login", model);

        if (response!.Succeeded) {
            await _localStorageService.SetItem("token", response.Token);

            var user = await _httpService.GetAsync<User>("api/user");
            await _localStorageService.SetItem("user", user);

            ((AuthProvider)_authProvider).LoggedIn(user!);
        }

        return response!;
    }

    public async Task LogoutAsync() {
        await _httpService.DeleteAsync("api/auth/login");
        await _localStorageService.Clear();
        ((AuthProvider)_authProvider).LoggedOut();
    }
}