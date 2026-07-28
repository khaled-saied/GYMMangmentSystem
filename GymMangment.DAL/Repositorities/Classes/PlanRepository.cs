using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.DbContexts;
using GymMangment.DAL.Models;
using GymMangment.DAL.Repositorities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymMangment.DAL.Repositorities.Classes
{
    public class PlanRepository : IPlanRepository
    {
        private readonly GymDbContext dbContext;

        public PlanRepository(GymDbContext gymDbContext) 
        {
            this.dbContext = gymDbContext;
        }

        public async Task<IEnumerable<Plan>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            //if(tracking)
            //{
            //    return await dbContext.Plans.ToListAsync(ct);
            //}
            //else
            //{
            //    return await dbContext.Plans.AsNoTracking().ToListAsync(ct);
            //}

            IQueryable<Plan> query = tracking ? dbContext.Plans : dbContext.Plans.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<int> AddAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Add(plan);
            return await dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> DeleteAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Remove(plan);
            return await dbContext.SaveChangesAsync(ct);
        }


        public async Task<Plan?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await dbContext.Plans.FindAsync(id);
        }

        public Task<int> UpdateAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Plans.Update(plan);
            return dbContext.SaveChangesAsync(ct);
        }
    }
}
