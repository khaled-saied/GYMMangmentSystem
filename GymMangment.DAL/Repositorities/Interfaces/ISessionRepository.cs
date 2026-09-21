using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Data.Models;

namespace GymMangment.DAL.Repositorities.Interfaces
{
    public interface ISessionRepository: IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct=default);
        Task<int> GetCountOfBookSlotsAsync(int sessionId,CancellationToken ct=default);
    }
}
