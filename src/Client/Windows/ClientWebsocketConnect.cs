
using System.Net.WebSockets;
using System.Text;

namespace Client.Windows;

public class ClientWebSocketConnection {
    public ClientWebSocket Socket { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }

    public event Action<string> OnMessageReceived = delegate { };
    public event Action OnDisconnect = delegate { };

    public ClientWebSocketConnection(ClientWebSocket socket) {
        Socket = socket;
        CancellationTokenSource = new CancellationTokenSource();
    }

    public async Task StartListeningAsync() {
        var buffer = new byte[1024];
        var result = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationTokenSource.Token);

        while (!result.CloseStatus.HasValue && !CancellationTokenSource.IsCancellationRequested) {
            result = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationTokenSource.Token);

            if (result.MessageType == WebSocketMessageType.Close) {
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            OnMessageReceived(message);
        }

        OnDisconnect();
    }

    public async Task SendMessageAsync(string message) {
        var buffer = Encoding.UTF8.GetBytes(message);
        await Socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationTokenSource.Token);
    }

    public async Task CloseAsync() {
        await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationTokenSource.Token);
    }

    public async Task Terminate() {
        await CloseAsync();
        CancellationTokenSource.Cancel();
    }
}