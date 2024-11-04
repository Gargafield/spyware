
using System.Net.WebSockets;
using System.Text;

namespace Client.Windows;

public class WebSocketConnection {
    public ClientWebSocket ClientWebSocket { get; set; }

    public WebSocketConnection() {
        ClientWebSocket = new ClientWebSocket();
    }

    public async Task ConnectAsync(Uri uri) {
        await ClientWebSocket.ConnectAsync(uri, CancellationToken.None);
    }

    public async Task<string> ReceiveAsync() {
        var buffer = new byte[1024];
        var result = await ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }

    public async Task SendAsync(string message) {
        var buffer = Encoding.UTF8.GetBytes(message);
        await ClientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task CloseAsync() {
        await ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }

    public void Dispose() {
        ClientWebSocket.Dispose();
    }
}