
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
public class UserController : ControllerBase {
    private readonly IUserService _userService;

    public UserController(IUserService userService) {
        _userService = userService;
    }

    [HttpGet("api/user")]
    [Authorize]
    public async Task<ActionResult<User>> GetCurrentUserAsync() {
        if (_userService.TryGetUser(User.Identity?.Name!, out User user)) {
            return Ok(user);
        }

        return Unauthorized();
    }
}