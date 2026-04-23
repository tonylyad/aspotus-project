using Aspotus.Gateway.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Gateway.Data.Context;

/// <summary>
/// Контекст базы данных Gateway.
/// </summary>
public class GatewayDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> options)
        : base(options)
    {
    }
}