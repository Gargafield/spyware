using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Server.Services;
using Shared;
using Shared.Models;

namespace Server.Utils;

public interface IWebSocketConnection {
    public WebSocket WebSocket { get; set; }
    public event Action<WebSocketConnection, string> OnMessage;
    public Task StartListening();
    public Task SendMessageAsync(string message);
}

public class WebSocketConnection : IWebSocketConnection, IDisposable {
    public WebSocket WebSocket { get; set; }
    public User? User { get; set; }

    private CancellationTokenSource _cancellationTokenSource;

    public event Action<WebSocketConnection, string> OnMessage = default!;
    public event Action<WebSocketConnection> OnDisconnected = default!;

    public WebSocketConnection(WebSocket webSocket) {
        WebSocket = webSocket;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task StartListening() {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        while (WebSocket.State == WebSocketState.Open && !_cancellationTokenSource.Token.IsCancellationRequested) {
            try {
                result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) {
                break;
            }
            catch (WebSocketException) {
                break;
            }

            if (result.MessageType == WebSocketMessageType.Close) {
                await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", _cancellationTokenSource.Token);
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            OnMessage?.Invoke(this, message);
        }

        OnDisconnected?.Invoke(this);
    }

    public async Task SendMessageAsync(string message) {
        await WebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
    }

    public async Task SendErrorAsync(string message, object? obj = null) {
        await SendMessageAsync(Json.Serialize(new ErrorMessage {
            Message = message,
            Object = obj
        }));
    }

    public WebSocketConnection Terminate() {
        WebSocket.Abort();
        _cancellationTokenSource.Cancel();
        return this;
    }

    public void Dispose() {
        WebSocket.Dispose();
    }
}