using System.Net.WebSockets;
using System.Text;

namespace Server.Utils;

public interface IWebSocketConnection {
    public string Username { get; set; }
    public WebSocket WebSocket { get; set; }
    public event EventHandler<string> OnMessage;
    public Task StartListening();
}

public class WebSocketConnection : IWebSocketConnection, IDisposable {
    public string Username { get; set; }
    public WebSocket WebSocket { get; set; }

    private CancellationTokenSource _cancellationTokenSource;

    public event EventHandler<string> OnMessage = default!;

    public WebSocketConnection(string username, WebSocket webSocket) {
        Username = username;
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
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            OnMessage?.Invoke(this, message);
        }
    }

    public async Task Send(byte[] message) {
        await WebSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
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