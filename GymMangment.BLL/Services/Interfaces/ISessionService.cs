using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.ViewModels.SessionViewModels;
using GymMangment.DAL.Data.Models;

namespace GymMangment.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default);
        Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);

        Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default);
    }
}
