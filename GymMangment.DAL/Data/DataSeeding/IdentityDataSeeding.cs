using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymMangment.DAL.Data.DataSeeding
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager
            , UserManager<ApplicationUser> userManager,
            ILogger logger,
            CancellationToken ct=default)
        {

            try
            {
                bool hasUser = await userManager.Users.AnyAsync(ct);
                bool hasRole = await roleManager.Roles.AnyAsync(ct);

                if (hasRole && hasUser)
                    return;

                var roles = new List<IdentityRole>
            {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin"),
            };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name!))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            logger.LogError($"Error creating role {role.Name}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                if (!hasUser)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Khaled",
                        LastName = "Selim",
                        UserName = "khaledselim",
                        Email = "Khaled070@gmail.com",
                        PhoneNumber = "01113739658",
                    };
                    await userManager.CreateAsync(MainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");

                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Ali",
                        LastName = "Selim",
                        UserName = "aliselim",
                        Email = "Ali070@gmail.com",
                        PhoneNumber = "01113732601",
                    };
                    await userManager.CreateAsync(Admin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");


                    logger.LogInformation("Default users and roles have been seeded successfully.");
                }

                return;
            }
            catch (Exception ex)
            {

                logger.LogError($"An error occurred while seeding identity data: {ex.Message}");
                return;
            }
        }
    }
}
