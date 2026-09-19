using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.PlanViewModels;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;

namespace GymMangment.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IGenericRepository<Plan> _planrepository;
        private readonly IGenericRepository<MemberShip> _membershipRepository;

        public PlanService(IGenericRepository<Plan> planrepository, IGenericRepository<MemberShip> membershipRepository)
        {
            this._planrepository = planrepository;
            this._membershipRepository = membershipRepository;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _planrepository.GetAllAsync(ct: ct);
            return plans.Select(p => new PlanViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DurationDays = p.DurationDays,
                Price = p.Price,
                IsActive = p.IsActive
            });
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _planrepository.GetByIdAsync(planId, ct: ct);
            if (plan == null)
                return null;
            else
            {
                return new PlanViewModel
                {
                    Name = plan.Name,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    Price = plan.Price,
                    IsActive = plan.IsActive
                };
            }
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _planrepository.GetByIdAsync(planId, ct: ct);
            if (plan is null || !plan.IsActive)
                return null;
            if(await HasActiveMembershipsAsync(planId,ct))
                return null;
            else
            {
                return new UpdatePlanViewModel
                {
                    PlanName = plan.Name,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    Price = plan.Price
                };
            }
        }

        public async Task<bool> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan =await _planrepository.GetByIdAsync(planId, ct);
            if (plan is null)
                return false;

            if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct))
                return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            var result = await _planrepository.UpdateAsync(plan, ct);
            return result > 0;
        }

        public async Task<bool> UpdatePlanAsync(int planId, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan =await _planrepository.GetByIdAsync(planId, ct: ct);
            if (plan is null || !plan.IsActive)
                return false;
            if(await HasActiveMembershipsAsync(planId,ct))
                return false;

            plan.Description = model.Description;
            plan.DurationDays = model.DurationDays;
            plan.Price = model.Price;
            plan.UpdatedAt = DateTime.Now;

            var result = await _planrepository.UpdateAsync(plan, ct);
            return result >0;
        }

        #region Helper Methods
        private async Task<bool> HasActiveMembershipsAsync (int planId, CancellationToken ct = default)
        {
            return await _membershipRepository.AnyAsync(m=>m.PlanId ==planId && m.EndDate>DateTime.Now,ct) ;
        }
        #endregion
    }
}
