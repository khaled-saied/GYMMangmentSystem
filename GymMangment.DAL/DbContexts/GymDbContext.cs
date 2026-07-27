using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Configurations;
using GymMangment.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMangment.DAL.DbContexts
{
    public class GymDbContext: DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlaneConfiguration());
        }

        public DbSet<Plan> Plans { get; set; }
    }
}
