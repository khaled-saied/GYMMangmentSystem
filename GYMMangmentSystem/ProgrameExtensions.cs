using GymMangment.DAL.Data.DataSeeding;
using GymMangment.DAL.Data.DbContexts;
using GymMangment.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GYMMangmentSystem.PL
{
    public static class ProgrameExtensions
    {
        public static async Task MigrateAndSeedDataBaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var pendingMigration = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigration.Any())
            {
                logger.LogInformation(
                    $"Applying {pendingMigration.Count()} Pending Migration");

                await dbContext.Database.MigrateAsync();
            }

            var seedFolderPath = Path.Combine(
                app.Environment.ContentRootPath,
                "wwwroot",
                "Files");

            await GymDataSeeding.SeedAsync(
                dbContext,
                seedFolderPath,
                logger);

            await IdentityDataSeeding.SeedIdentityDataAsync(roleManager,userManager,logger);
        }
    }
}
