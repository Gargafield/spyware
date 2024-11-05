using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models;

namespace Web.Services;

public interface IAuthService {
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

    public async Task<LoginResultModel> LoginAsync(LoginModel model) {
        var response = await _httpService.PostAsync<LoginResultModel>("api/auth/login", model);

        if (response!.Succeeded) {
            await _localStorageService.SetItem("token", response.Token);
            ((AuthProvider)_authProvider).LoggedIn(response.Token);
        }

        return response!;
    }

    public async Task LogoutAsync() {
        await _httpService.DeleteAsync("api/auth/login");
        await _localStorageService.RemoveItem("token");
        ((AuthProvider)_authProvider).LoggedOut();
    }
}