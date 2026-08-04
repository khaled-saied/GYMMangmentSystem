using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Data.DbContexts;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymMangment.DAL.Repositorities.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(GymDbContext DbContext)
        {
            _dbContext = DbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }
        public async Task<int> AddAsync(TEntity entity)
        {
            _dbSet.Add(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _dbSet: _dbSet.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet.FindAsync(id, ct);
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return _dbContext.SaveChangesAsync();
        }
    }
}
