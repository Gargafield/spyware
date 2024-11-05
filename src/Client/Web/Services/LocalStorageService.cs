
using System.Text.Json;
using Microsoft.JSInterop;

namespace Web.Services;

public interface ILocalStorageService {
    Task<T> GetItem<T>(string key);
    Task SetItem<T>(string key, T value);
    Task RemoveItem(string key);
    Task Clear();
    Task<bool> ContainsKey(string key);
}

public class LocalStorageService : ILocalStorageService {
    private readonly IJSRuntime JSRuntime;

    public LocalStorageService(IJSRuntime jsRuntime) {
        JSRuntime = jsRuntime;
    }

    public async Task<T> GetItem<T>(string key) {
        var value = await JSRuntime.InvokeAsync<string>("localStorage.getItem", key);
        return JsonSerializer.Deserialize<T>(value)!;
    }

    public async Task SetItem<T>(string key, T value) {
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
    }

    public async Task RemoveItem(string key) {
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }

    public async Task Clear() {
        await JSRuntime.InvokeVoidAsync("localStorage.clear");
    }

    public async Task<bool> ContainsKey(string key) {
        var value = await JSRuntime.InvokeAsync<string>("localStorage.getItem", key);
        return value != null;
    }
}