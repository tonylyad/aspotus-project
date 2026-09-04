using Aspotus.Gateway.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Aspotus.Gateway.Data.Seed;

/// <summary>
/// Начальное заполнение базы Gateway.
/// </summary>
public static class GatewaySeedData
{
    /// <summary>
    /// Выполняет начальное заполнение ролей и тестовых пользователей.
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

        var seedUsers = new[]
        {
            new SeedUser("customer", "customer@aspotus.com", "Тестовый покупатель", "Customer"),
            new SeedUser("moderator", "moderator@aspotus.com", "Модератор контента", "ContentModerator"),
            new SeedUser("operator", "operator@aspotus.com", "Оператор заказов", "Operator"),
            new SeedUser("admin", "admin@aspotus.com", "Главный админ", "Admin")
        };

        foreach (var seedUser in seedUsers)
        {
            await EnsureUserAsync(userManager, seedUser);
        }
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        SeedUser seedUser)
    {
        var user = await userManager.FindByNameAsync(seedUser.Login) ??
                   await userManager.FindByEmailAsync(seedUser.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = seedUser.Login,
                Email = seedUser.Email,
                FullName = seedUser.FullName
            };

            var createResult = await userManager.CreateAsync(user, SeedUser.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException(
                    $"Не удалось создать seed-пользователя '{seedUser.Login}': {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, seedUser.Role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, seedUser.Role);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException(
                    $"Не удалось назначить роль '{seedUser.Role}' пользователю '{seedUser.Login}': {errors}");
            }
        }
    }

    private sealed record SeedUser(string Login, string Email, string FullName, string Role)
    {
        public const string Password = "123456";
    }
}
