using Microsoft.AspNetCore.Identity;

namespace Aspotus.Gateway.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}