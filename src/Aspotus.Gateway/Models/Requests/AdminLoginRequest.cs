using System.ComponentModel.DataAnnotations;

namespace Aspotus.Gateway.Models.Requests;

public class AdminLoginRequest
{
    [Required]
    public string Login { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
