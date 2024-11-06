
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Shared;

namespace Web.Services;

public interface IHttpService {
    Task<T?> GetAsync<T>(string uri);
    Task GetAsync(string uri);
    Task<T?> PostAsync<T>(string uri, object data);
    Task<T?> PutAsync<T>(string uri, object data);
    Task DeleteAsync(string uri);
}

public class HttpService : IHttpService {
    private readonly HttpClient _client;
    private readonly ILocalStorageService _localStorage;

    public HttpService(HttpClient client, ILocalStorageService localStorage) {
        _client = client;
        _localStorage = localStorage;
    }

    public async Task GetAsync(string uri) {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await sendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T?> GetAsync<T>(string uri) {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await sendAsync(request);
        return await response.Content.ReadFromJsonAsync<T>()!;
    }

    public async Task<T?> PostAsync<T>(string uri, object data) {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = new StringContent(Json.Serialize(data), Encoding.UTF8, "application/json");
        var response = await sendAsync(request);
        return await response.Content.ReadFromJsonAsync<T>()!;
    }

    public async Task<T?> PutAsync<T>(string uri, object data) {
        var request = new HttpRequestMessage(HttpMethod.Put, uri);
        request.Content = new StringContent(Json.Serialize(data), Encoding.UTF8, "application/json");
        var response = await sendAsync(request);
        return await response.Content.ReadFromJsonAsync<T>()!;
    }

    public async Task DeleteAsync(string uri) {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        var response = await sendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpResponseMessage> sendAsync(HttpRequestMessage request) {
        if (await _localStorage.ContainsKey("token"))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _localStorage.GetItem<string>("token"));

        var response = await _client.SendAsync(request);
        return response;
    }
}