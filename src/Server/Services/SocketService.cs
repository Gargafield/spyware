using System.Net.WebSockets;
using System.Text;
using Server.Utils;

namespace Server.Services;

public interface ISocketService {
    public Task HandleConnection(WebSocket webSocket);
}

public class SocketService : ISocketService {
    private IConfiguration _config;

    public SocketService(IConfiguration config) {
        _config = config;
    }

    public async Task HandleConnection(WebSocket webSocket) {
        var connection = new WebSocketConnection(webSocket);
        connection.OnMessage += async (sender, message) => {
            var response = Encoding.UTF8.GetBytes($"Received: {message}");
            await connection.Send(response);
        };

        await connection.StartListening();

        connection.Dispose();
    }
}