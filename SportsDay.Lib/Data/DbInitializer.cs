using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SportsDay.Lib.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var sportsDayDbContext = scope.ServiceProvider.GetRequiredService<SportsDayDbContext>();
                
                // Apply any pending migrations
                try
                {
                    await sportsDayDbContext.Database.EnsureCreatedAsync();
                    await sportsDayDbContext.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }

                // Execute SQL script from setup.sql
                /* var sqlScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQL", "setup.sql");
                 var sqlScript = File.ReadAllText(sqlScriptPath);
                 context.Database.ExecuteSqlRaw(sqlScript);*/

                // Seed initial data if needed
                try
                {
                    // Seed default admin user
                    await SeedDefaultRolesAsync(scope.ServiceProvider);
                    await SeedDefaultUsersAsync(scope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }

        private static async Task SeedDefaultRolesAsync(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create default roles if they don't exist
            string[] roles = { "Administrator", "Judge", "Announcer", "HouseLeader", "Viewer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
        private static async Task SeedDefaultUsersAsync(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var defaultUsers = config.GetValue<DefaultUser[]>("DefaultUsers");
            foreach (var defaultUser in defaultUsers)
            {
                if (string.IsNullOrEmpty(defaultUser.Email))
                {
                    continue;
                }
                // Create default user if it doesn't exist
                if (await userManager.FindByEmailAsync(defaultUser.Email) == null)
                {
                    if (string.IsNullOrEmpty(defaultUser.Password))
                    {
                        defaultUser.Password = "sdm-admin2025";
                    }
                    if (string.IsNullOrEmpty(defaultUser.UserName))
                    {
                        var i = defaultUser.Email.IndexOf("@");
                        defaultUser.UserName = defaultUser.Email.Substring(0, i);
                    }

                    var basicUser = new IdentityUser { UserName = defaultUser.UserName, Email = defaultUser.Email, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(basicUser, defaultUser.Password);
                    if (result.Succeeded == false)
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(defaultUser.Role))
                    {
                        defaultUser.Role = "Viewer";
                    }
                    await userManager.AddToRoleAsync(basicUser, defaultUser.Role);

                }
            }
        }

    }

    internal class DefaultUser
    {
        public string? Email { get; internal set; }
        public string? UserName { get; internal set; }
        public string? Password { get; internal set; }
        public string? Role { get; internal set; }
    }
}
