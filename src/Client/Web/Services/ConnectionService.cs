
using System.Collections;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Shared;
using Shared.Models;

namespace Web.Services;

public interface IConnectionService
{
    Task<Connection> ConnectAsync();
    Task SendScreenStatusAsync(bool turnedOn);

    public event Action<Student> OnStudentAdded;
    public event Action<int> OnHandRaised;
    public event Action<int> OnHandLowered;
}

public class ConnectionService : IConnectionService {
    private readonly IConfiguration _configuration;
    private readonly ILocalStorageService _localStorageService;
    private readonly IAuthService _authService;
    private Connection? _connection;

    public event Action<Student> OnStudentAdded = delegate { };
    public event Action<int> OnHandRaised = delegate { };
    public event Action<int> OnHandLowered = delegate { };

    public ConnectionService(IConfiguration configuration, ILocalStorageService localStorageService, IAuthService authService) {
        _configuration = configuration;
        _localStorageService = localStorageService;
        _authService = authService;

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    }

    public async Task<Connection> ConnectAsync() {
        if (!await _authService.IsAuthenticated())
            throw new Exception("User is not authenticated");

        var socket = new ClientWebSocket();
        var cancellationTokenSource = new CancellationTokenSource();
        
        var wsUrl = _configuration["API_URL"]!.Replace("http", "ws");
        var uri = new Uri(wsUrl + "/api/connect");
        await socket.ConnectAsync(uri, cancellationTokenSource.Token);

        _connection = new Connection(socket, cancellationTokenSource);

        await _connection.SendMessageAsync(Json.Serialize(new AuthMessage {
            AccessToken = await _localStorageService.GetItem<string>("token")
        }));

        await foreach (var message in _connection.StartListeningAsync()) {
            _ = OnMessageReceived(message);
            Console.WriteLine(message);
        }

        return _connection;
    }

    private async Task OnMessageReceived(string message) {
        var messageObject = Json.Deserialize<Message>(message);

        if (messageObject is StudentAddedMessage studentAddedMessage) {
            OnStudentAdded(studentAddedMessage.Student);
        } else if (messageObject is HandStatusMessage handStatusMessage) {
            if (handStatusMessage.Raised) {
                OnHandRaised(handStatusMessage.StudentId);
            } else {
                OnHandLowered(handStatusMessage.StudentId);
            }
        } else if (messageObject is ScreenStatusMessage screenStatusMessage) {
            // Do nothing
        }
    }

    public async Task SendScreenStatusAsync(bool turnedOn) {
        if (_connection == null) {
            return;
        }

        var message = new ScreenStatusMessage {
            TurnedOn = turnedOn
        };
        var messageJson = Json.Serialize(message);

        await _connection.SendMessageAsync(messageJson);
    }
}

public class Connection {
    private readonly ClientWebSocket _socket;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public Connection(ClientWebSocket socket, CancellationTokenSource cancellationTokenSource) {
        _socket = socket;
        _cancellationTokenSource = cancellationTokenSource;
    }

    public async Task SendMessageAsync(string message)
    {
        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        await _socket.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, _cancellationTokenSource.Token);
    }

    public async IAsyncEnumerable<string> StartListeningAsync() {
        var buffer = new ArraySegment<byte>(new byte[1024]);
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            WebSocketReceiveResult result;
            using var stream = new MemoryStream();
            do
            {
                result = await _socket.ReceiveAsync(buffer, _cancellationTokenSource.Token);
                stream.Write(buffer.Array, buffer.Offset, result.Count);
            } while (!result.EndOfMessage);


            stream.Seek(0, SeekOrigin.Begin);

            yield return Encoding.UTF8.GetString(stream.ToArray());

            if (result.MessageType == WebSocketMessageType.Close)
                break;
        }
    }
}