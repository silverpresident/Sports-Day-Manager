using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsDay.Lib.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SportsDay.Web.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var sportsDayDbContext = scope.ServiceProvider.GetRequiredService<SportsDayDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Apply any pending migrations
                sportsDayDbContext.Database.Migrate();

                // Execute SQL script from setup.sql
               /* var sqlScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQL", "setup.sql");
                var sqlScript = File.ReadAllText(sqlScriptPath);
                context.Database.ExecuteSqlRaw(sqlScript);*/

                // Create default roles if they don't exist
                string[] roles = { "Administrator","Judge", "Viewer" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Create default admin user if it doesn't exist
                if (await userManager.FindByEmailAsync("silverpresident@gmail.com") == null)
                {
                    var adminUser = new IdentityUser { UserName = "admin", Email = "silverpresident@gmail.com", EmailConfirmed = true };
                    var result = await userManager.CreateAsync(adminUser, "admin123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Administrator");
                    }
                }

                // Create default user if it doesn't exist
                 if (await userManager.FindByEmailAsync("support@stjago.edu.jm") == null)
                {
                    var basicUser = new IdentityUser { UserName = "user", Email = "support@stjago.edu.jm", EmailConfirmed = true };
                    var result = await userManager.CreateAsync(basicUser, "admin123");
                     if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(basicUser, "Administrator");
                    }
                }
            }
        }
    }
}
