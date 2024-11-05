
using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class LoginModel {
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = "";
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public class LoginResultModel {
    public bool Succeeded { get; set; }
    public string Token { get; set; } = "";
    public string Message { get; set; } = "";
}