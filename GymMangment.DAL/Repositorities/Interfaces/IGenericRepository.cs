using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Data.Models;

namespace GymMangment.DAL.Repositorities.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity , new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);

        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<int> AddAsync(TEntity entity);

        Task<int> UpdateAsync(TEntity entity, CancellationToken ct = default);

        Task<int> DeleteAsync(TEntity entity);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

        Task<TEntity?> FirstOrDefultAsync(Expression<Func<TEntity, bool>> predicate,bool tracking=false, CancellationToken ct = default);
    }
}
