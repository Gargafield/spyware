using System.Net.WebSockets;
using System.Text.Json;
using Server.Utils;
using Shared;
using Shared.Models;

namespace Server.Services;

public interface ISocketService {
    public Task HandleConnection(WebSocket webSocket);
}

public class SocketService : ISocketService {
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IClassService _classService;
    public Dictionary<int, WebSocketConnection> Connections { get; } = new();

    public SocketService(IUserService userService, IAuthService authService, IClassService classService) {
        _userService = userService;
        _authService = authService;
        _classService = classService;
    }

    public async Task HandleConnection(WebSocket webSocket) {
        var connection = new WebSocketConnection(webSocket);
        connection.OnMessage += OnMessageReceived;
        connection.OnDisconnected += (connection) => {
            if (connection.User is not null) {
                Connections.Remove(connection.User.Id);
            }
        };

        await connection.StartListening();

        connection.Dispose();
    }

    private async void OnMessageReceived(WebSocketConnection connection, string message) {
        var messageObject = Json.Deserialize<Message>(message);

        if (messageObject is null) {
            await connection.SendErrorAsync("Invalid message format", message);
            return;
        }

        switch (messageObject) {
            case AuthMessage authMessage:
                await HandleAuthMessage(connection, authMessage);
                break;
            case ScreenStatusMessage screenStatusMessage:
                await HandleScreenStatusMessage(connection, screenStatusMessage);
                break;
            case HandStatusMessage handStatusMessage:
                await HandleHandStatusMessage(connection, handStatusMessage);
                break;
            case ErrorMessage:
                break;
            default:
                await connection.SendErrorAsync("Unknown message type", message);
                break;
        }
    }

    private async Task HandleAuthMessage(WebSocketConnection connection, AuthMessage authMessage) {
        if (!_authService.ValidateToken(authMessage.AccessToken, out var username)) {
            await connection.SendErrorAsync("Invalid token", authMessage);
            return;
        }

        if (!_userService.TryGetUser(username, out var user)) {
            await connection.SendErrorAsync("User not found", authMessage);
            return;
        }

        if (Connections.ContainsKey(user.Id)) {
            await connection.SendErrorAsync("User already connected", authMessage);
            return;
        }

        connection.User = user;
        Connections.Add(user.Id, connection);

        // if (user.Role == "Student") {
        //     var Class = await _classService.GetClassForStudentAsync(user.Id);
        //     if (Connections.TryGetValue(Class.TeacherId, out var teacherConnection)) {
        //         var message = new StudentAddedMessage() {
        //             Student = new Student {
        //                 Id = user.Id,
        //                 Username = user.Username
        //             }
        //         };
                
        //         await teacherConnection.SendMessageAsync(
        //             Json.Serialize(message)
        //         );
        //     }
        // }
    }

    private async Task HandleScreenStatusMessage(WebSocketConnection connection, ScreenStatusMessage screenStatusMessage) {
        if (connection.User is null) {
            await connection.SendErrorAsync("User not authenticated", screenStatusMessage);
            return;
        }

        if (connection.User.Role != "Teacher") {
            await connection.SendErrorAsync("User is not a teacher", screenStatusMessage);
            return;
        }

        var Class = await _classService.GetClassForTeacherAsync(connection.User.Id);
        if (Class is null) {
            await connection.SendErrorAsync("Teacher is not in a class", screenStatusMessage);
            return;
        }



        foreach (var student in Class.Students) {
            if (Connections.TryGetValue(student.Id, out var studentConnection)) {
                await studentConnection.SendMessageAsync(
                    Json.Serialize(screenStatusMessage)
                );
            }
        }
    }

    private async Task HandleHandStatusMessage(WebSocketConnection connection, HandStatusMessage handStatusMessage) {
        if (connection.User is null) {
            await connection.SendErrorAsync("User not authenticated", handStatusMessage);
            return;
        }

        if (connection.User.Role != "Student") {
            await connection.SendErrorAsync("User is not a student", handStatusMessage);
            return;
        }

        var Class = await _classService.GetClassForStudentAsync(connection.User.Id);
        if (Class is null) {
            await connection.SendErrorAsync("Student is not in a class", handStatusMessage);
            return;
        }

        if (Connections.TryGetValue(Class.TeacherId, out var teacherConnection)) {
            await teacherConnection.SendMessageAsync(
                Json.Serialize(new HandStatusMessage() {
                    StudentId = connection.User.Id,
                    Raised = handStatusMessage.Raised
                })
            );
        }
    }
}