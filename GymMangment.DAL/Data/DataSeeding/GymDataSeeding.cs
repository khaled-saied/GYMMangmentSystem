using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GymMangment.DAL.Data.DbContexts;
using GymMangment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymMangment.DAL.Data.DataSeeding
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext,string seedFolferPath,ILogger logger,CancellationToken ct=default)
        {
            try
            {
                if(! await dbContext.Plans.AnyAsync())
                {

                    var plans = LoadDataFromJSONFile<Plan>(seedFolferPath, "plans.json");

                    if (plans.Any())
                    {
                        dbContext.AddRange(plans);
                        
                        logger.LogInformation($"Plans Seeded With Count = {plans.Count}");
                    }

                    if (dbContext.ChangeTracker.HasChanges())
                        await dbContext.SaveChangesAsync();
                    else
                        logger.LogInformation("Plan Aready Seeded");
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Gym Data Seeding Fail");
                throw;
            }
        }

        private static List<T> LoadDataFromJSONFile<T>(string folderPath,string fileName)
        {
            var filePath = Path.Combine(folderPath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed Date File Not Found : {filePath}");
            var data = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
    }
}
