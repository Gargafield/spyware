
using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class RefreshModel {
    [Required]
    public string AccessToken { get; set; }
}