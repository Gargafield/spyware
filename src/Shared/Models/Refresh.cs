
using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class RefreshModel {
    [Required]
    public required string AccessToken { get; set; }
}