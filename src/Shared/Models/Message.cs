
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Models;

public enum MessageType
{
    StudentAdded,
    HandStatus,
    ScreenStatus,
    Auth
}

[JsonConverter(typeof(JsonMessageConverter))]
public abstract class Message {
    [JsonPropertyName("t")]
    public abstract MessageType Type { get; }
}

public class StudentAddedMessage : Message {
    [JsonPropertyName("t")]
    public override MessageType Type => MessageType.StudentAdded;

    [JsonPropertyName("student")]
    public Student Student { get; set; }
}

public class HandStatusMessage : Message {
    [JsonPropertyName("t")]
    public override MessageType Type => MessageType.HandStatus;

    [JsonPropertyName("studentId")]
    public int StudentId { get; set; }
    [JsonPropertyName("raised")]
    public bool Raised { get; set; }
}

public class ScreenStatusMessage : Message {
    [JsonPropertyName("t")]
    public override MessageType Type => MessageType.ScreenStatus;

    [JsonPropertyName("turnedOn")]
    public bool TurnedOn { get; set; }
}

public class AuthMessage : Message {
    [JsonPropertyName("t")]
    public override MessageType Type => MessageType.Auth;

    [JsonPropertyName("token")]
    public string AccessToken { get; set; }
}

public class JsonMessageConverter : JsonConverter<Message> {
    public override Message Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var root = jsonDocument.RootElement;

        var type = (MessageType)root.GetProperty("t").GetInt32();
        return type switch {
            MessageType.StudentAdded => JsonSerializer.Deserialize<StudentAddedMessage>(root.GetRawText(), options)!,
            MessageType.HandStatus => JsonSerializer.Deserialize<HandStatusMessage>(root.GetRawText(), options)!,
            MessageType.ScreenStatus => JsonSerializer.Deserialize<ScreenStatusMessage>(root.GetRawText(), options)!,
            MessageType.Auth => JsonSerializer.Deserialize<AuthMessage>(root.GetRawText(), options)!,
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options) {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}