using System.Net.WebSockets;
using System.Text;
using Server.Utils;

namespace Server.Services;

public interface ISocketService {
    public bool HasConnection(string username);
    public bool TryGetConnection(string username, out WebSocketConnection connection);
    public Task HandleConnection(WebSocket webSocket, string username);
}

public class SocketService : ISocketService {
    private static Dictionary<string, WebSocketConnection> _socketStore = new();
    private IConfiguration _config;

    public SocketService(IConfiguration config) {
        _config = config;
    }

    public bool HasConnection(string username) {
        return _socketStore.ContainsKey(username);
    }

    public bool TryGetConnection(string username, out WebSocketConnection connection) {
        return _socketStore.TryGetValue(username, out connection!);
    }

    public async Task HandleConnection(WebSocket webSocket, string username) {
        var connection = new WebSocketConnection(username, webSocket);
        _socketStore.Add(username, connection);
        connection.OnMessage += async (sender, message) => {
            var connection = (WebSocketConnection) sender!;
            var response = Encoding.UTF8.GetBytes($"Received: {message}");
            await connection.Send(response);
        };

        await connection.StartListening();

        _socketStore.Remove(username);
        connection.Dispose();
    }
}