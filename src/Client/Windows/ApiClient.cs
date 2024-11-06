
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared;
using Shared.Models;

namespace Client.Windows;

public class ApiClient {
    const string API_URL = "http://localhost:5167";
    const string WS_URL = "ws://localhost:5167";

    private CookieContainer cookieContainer;
    private HttpClientHandler httpClientHandler;
    private HttpClient httpClient;

    public ApiClient() {
        cookieContainer = new CookieContainer();
        httpClientHandler = new HttpClientHandler {
            CookieContainer = cookieContainer
        };
        httpClient = new HttpClient(httpClientHandler);
    }

    private void SetAccessToken(LoginResultModel token) {
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
    }

    public async Task<string> LoginAsync(string username, string password) {
        try {
            var response = await httpClient.PostAsync(
                $"{API_URL}/api/auth/login",
                new StringContent(Json.Serialize(new { username, password }), Encoding.UTF8, "application/json")
            ).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var accessToken = Json.Deserialize<LoginResultModel>(content)!;
            SetAccessToken(accessToken);
            return accessToken.Token;
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
            return string.Empty;
        }
    }

    public async Task<string> RefreshTokenAsync(string refreshToken) {
        var response = await httpClient.PutAsync(
            $"{API_URL}/api/auth/login",
            new StringContent(Json.Serialize(new { refreshToken }), Encoding.UTF8, "application/json")
        );
        var content = await response.Content.ReadAsStringAsync();
        var accessToken = Json.Deserialize<LoginResultModel>(content)!;
        SetAccessToken(accessToken);
        return accessToken.Token;
    }

    public async Task LogoutAsync() {
        await httpClient.DeleteAsync($"{API_URL}/api/auth/logout").ConfigureAwait(false);
    }

    public async Task<ClientWebSocketConnection> ConnectAsync() {
        var socket = new ClientWebSocket();
        await socket.ConnectAsync(new Uri($"{WS_URL}/api/connect"), CancellationToken.None)
            .ConfigureAwait(false);
        return new ClientWebSocketConnection(socket);
    }
}