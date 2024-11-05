
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Models;

[JsonConverter(typeof(JsonUserConverter))]
public abstract class User {
    public int Id { get; set; }
    public string Username { get; set; }
    public abstract string Role { get; }
}

public class JsonUserConverter : JsonConverter<User> {
    public override User? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        
        if (!root.TryGetProperty("Role", out var roleProperty) && !root.TryGetProperty("role", out roleProperty)) {
            throw new JsonException();
        }

        var role = roleProperty.GetString();
        return role switch {
            "Teacher" => JsonSerializer.Deserialize<Teacher>(root.GetRawText(), options),
            "Student" => JsonSerializer.Deserialize<Student>(root.GetRawText(), options),
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options) {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
