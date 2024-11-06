
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared;

public static class Json {
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new() {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, JsonSerializerOptions);
    public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}