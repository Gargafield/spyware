using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

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
            var websocket = await context.WebSockets.AcceptWebSocketAsync();
            await _socketService.HandleConnection(websocket).ConfigureAwait(false);

            return new EmptyResult();
        }

        return new StatusCodeResult((int) HttpStatusCode.BadRequest);
    }
}
