using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Data.Models;

namespace GymMangment.DAL.Repositorities.Interfaces
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAllAsync(bool tracking=false , CancellationToken ct = default);

        Task<Plan?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<int> AddAsync(Plan plan, CancellationToken ct = default);

        Task<int> UpdateAsync(Plan plan, CancellationToken ct = default);

        Task<int> DeleteAsync(Plan plan, CancellationToken ct = default);
    }
}
