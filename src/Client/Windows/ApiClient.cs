
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.Windows;

public class TokenModel {
    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; set; }
}

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

    private void SetAccessToken(TokenModel token) {
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
    }

    public async Task<string> LoginAsync(string username, string password) {
        try {
            var response = await httpClient.PostAsync(
                $"{API_URL}/api/auth/login",
                new StringContent(JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json")
            ).ConfigureAwait(false);
            var accessToken = JsonSerializer.Deserialize<TokenModel>( await response.Content.ReadAsStringAsync().ConfigureAwait(false))!;
            SetAccessToken(accessToken);
            return accessToken.AccessToken;
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
            return string.Empty;
        }
    }

    public async Task<string> RefreshTokenAsync(string refreshToken) {
        var response = await httpClient.PutAsync(
            $"{API_URL}/api/auth/login",
            new StringContent(JsonSerializer.Serialize(new { refreshToken }), Encoding.UTF8, "application/json")
        );
        var accessToken = JsonSerializer.Deserialize<TokenModel>(await response.Content.ReadAsStringAsync())!;
        SetAccessToken(accessToken);
        return accessToken.AccessToken;
    }

    public async Task LogoutAsync() {
        await httpClient.DeleteAsync($"{API_URL}/api/auth/logout").ConfigureAwait(false);
    }

    public async Task<ClientWebSocket> ConnectAsync() {
        var socket = new ClientWebSocket();
        socket.Options.Cookies = cookieContainer;
        socket.Options.SetRequestHeader("Authorization", httpClient.DefaultRequestHeaders.Authorization!.Parameter);
        await socket.ConnectAsync(new Uri($"{WS_URL}/api/connect"), CancellationToken.None)
            .ConfigureAwait(false);
        return socket;
    }
}