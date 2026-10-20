using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RazorPagesCar.Data;

public static class IdentitySeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration cfg)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        var email = cfg["Admin:Email"] ?? "admin@example.com";
        var pass = cfg["Admin:Password"] ?? "Admin#12345";
        const string role = "Admin";

        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            var create = await userManager.CreateAsync(user, pass);
            if (!create.Succeeded)
            {
                throw new Exception("Admin create failed: " +
                    string.Join("; ", create.Errors.Select(e => $"{e.Code}:{e.Description}")));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
