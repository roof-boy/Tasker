using Microsoft.AspNetCore.Identity;
using System;
using Tasker.API.Models.Database;

namespace Tasker.API.Data
{
    public class SeedingScripts
    {
        public static async Task SeedBasicRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                string[] roleNames = { "Administrator", "Team Leader", "User" };

                foreach (var roleName in roleNames) { 
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }
        }

        public static async Task SeedAdminUserAsync(UserManager<TaskerUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            // Check if admin user exists in the database
            if (await userManager.FindByNameAsync("Admin") == null)
            {
                var settings = configuration.GetSection("AdminAccountSettings");

                TaskerUser adminUser = new TaskerUser
                {
                    Email = settings["Email"],
                    UserName = settings["Username"],
                };

                adminUser.EmailConfirmed = true;
                adminUser.Activated = true;

                // TODO: Remove hard-coded password and read a hashed password from appsettings.json
                if (await userManager.CreateAsync(adminUser, settings["Password"] ?? throw new Exception("You must provide a default administrator password!")) == IdentityResult.Success)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                    await userManager.AddToRoleAsync(adminUser, "User");
                }
            }
        }
    }
}
