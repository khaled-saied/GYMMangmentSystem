using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Common;
using GymMangment.BLL.ViewModels.PlanViewModels;

namespace GymMangment.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct=default);
        Task<Result<PlanViewModel?>> GetPlanByIdAsync(int planId, CancellationToken ct=default);
        Task<Result<UpdatePlanViewModel?>> GetPlanToUpdateAsync(int planId, CancellationToken ct=default);
        Task<Result> UpdatePlanAsync(int planId, UpdatePlanViewModel model, CancellationToken ct=default);
        Task<Result> ToggleActivationAsync(int planId, CancellationToken ct=default);
    }
}
