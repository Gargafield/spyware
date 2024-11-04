using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Auth;
using Server.Models;
using Server.Services;
using Server.Utils;

namespace Server.Controllers;

public class ConnectionController : ControllerBase
{
    private ISocketService _socketService;
    private IAuthService _authService;
    private readonly ILogger<ConnectionController> _logger;

    public ConnectionController(ISocketService socketService, IAuthService authService, ILogger<ConnectionController> logger)
    {
        _socketService = socketService;
        _authService = authService;
        _logger = logger;
    }

    [Route("api/connect")]
    [AllowAnonymous]
    public async Task<IActionResult> Connect() {

        var context = ControllerContext.HttpContext;

        if (context.WebSockets.IsWebSocketRequest) {
            var token = AuthHandler.GetToken(Request);

            if (!_authService.ValidateToken(token!, out string username))
                return new StatusCodeResult((int) HttpStatusCode.Unauthorized);
            
            if (_socketService.TryGetConnection(username, out WebSocketConnection socket)) {
                socket.Terminate();
            }

            var websocket = await context.WebSockets.AcceptWebSocketAsync();
            await _socketService.HandleConnection(websocket, username).ConfigureAwait(false);

            return new EmptyResult();
        }

        return new StatusCodeResult((int) HttpStatusCode.BadRequest);
    }
}
