using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassController : ControllerBase {
    private readonly IClassService _classService;
    private readonly IUserService _userService;

    public ClassController(IClassService classService, IUserService userService) {
        _classService = classService;
        _userService = userService;
    }

    [HttpGet("current")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<Class>> GetCurrentClassAsync() {
        if (_userService.TryGetUser(User.Identity?.Name!, out User user)
            && user is Teacher) {
            var currentClass = await _classService.GetClassForTeacherAsync(user.Id);
            return Ok(currentClass);
        }

        return Unauthorized();
    }
}