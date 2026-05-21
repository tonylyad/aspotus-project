using Aspotus.Gateway.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Aspotus.Gateway.Data.Seed;

/// <summary>
/// Начальное заполнение базы Gateway.
/// </summary>
public static class GatewaySeedData
{
    /// <summary>
    /// Выполняет начальное заполнение ролей и администратора.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roles = new List<ApplicationRole>
        {
            new() { Name = "Customer", Description = "Покупатель" },
            new() { Name = "ContentModerator", Description = "Модератор контента" },
            new() { Name = "Operator", Description = "Оператор заказов" },
            new() { Name = "Admin", Description = "Администратор системы" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }

        const string adminLogin = "admin";
        const string adminEmail = "admin@aspotus.com";
        const string adminPassword = "123456";

        var admin = await userManager.FindByNameAsync(adminLogin);

        if (admin is null)
        {
            admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin is not null)
            {
                admin.UserName = adminLogin;
                admin.FullName = "Главный админ";
                await userManager.UpdateAsync(admin);
            }
            else
            {
                admin = new ApplicationUser
                {
                    UserName = adminLogin,
                    Email = adminEmail,
                    FullName = "Главный админ"
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
            }
        }
        else
        {
            admin.FullName = "Главный админ";
            await userManager.UpdateAsync(admin);
        }

        if (admin is not null && !await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}