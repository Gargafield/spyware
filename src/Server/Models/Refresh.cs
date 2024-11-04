
using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class RefreshModel {
    [Required]
    public required string AccessToken { get; set; }
}