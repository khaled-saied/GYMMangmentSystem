using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Common;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.PlanViewModels;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;

namespace GymMangment.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return Result<IEnumerable<PlanViewModel>>.Ok(plans.Select(p => new PlanViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DurationDays = p.DurationDays,
                Price = p.Price,
                IsActive = p.IsActive
            }));
        }

        public async Task<Result<PlanViewModel?>> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct: ct);
            if (plan == null)
                return Result<PlanViewModel?>.NotFound("Plan not found");
            else
            {
                return Result<PlanViewModel?>.Ok(new PlanViewModel
                {
                    Name = plan.Name,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    Price = plan.Price,
                    IsActive = plan.IsActive
                });
            }
        }

        public async Task<Result<UpdatePlanViewModel?>> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct: ct);
            if (plan is null || !plan.IsActive)
                return Result<UpdatePlanViewModel?>.NotFound("Plan not found");
            if(await HasActiveMembershipsAsync(planId,ct))
                return Result<UpdatePlanViewModel?>.Fail("Plan has active memberships");
            else
            {
                return Result<UpdatePlanViewModel?>.Ok( new UpdatePlanViewModel
                {
                    PlanName = plan.Name,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    Price = plan.Price
                });
            }
        }

        public async Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan =await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan is null)
                return Result.NotFound("Plan not found");
            if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct))
                return Result.Validation("Plan has active memberships");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

             _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to toggle plan activation");
        }

        public async Task<Result> UpdatePlanAsync(int planId, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan =await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct: ct);
            if (plan is null || !plan.IsActive)
                return Result.NotFound("Plan not found");
            if(await HasActiveMembershipsAsync(planId,ct))
                return Result.Validation("Plan has active memberships");

            plan.Description = model.Description;
            plan.DurationDays = model.DurationDays;
            plan.Price = model.Price;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update plan");
        }

        #region Helper Methods
        private async Task<bool> HasActiveMembershipsAsync (int planId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<MemberShip>().AnyAsync(m=>m.PlanId ==planId && m.EndDate>DateTime.Now,ct) ;
        }
        #endregion
    }
}
