using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.PlanViewModels;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GYMMangmentSystem.PL.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService _service;

        public PlansController(IPlanService service)
        {
            this._service = service;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _service.GetAllPlansAsync(ct);
            return View(plans);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _service.GetPlanByIdAsync(id, ct);
            if (plan == null)
                return RedirectToAction(nameof(Index));
            return View(plan);
        }

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await _service.GetPlanToUpdateAsync(id, ct);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _service.UpdatePlanAsync(id, model, ct);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to update the plan.";
                return View(model);
            }
            TempData["SuccessMessage"] = "Plan updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await _service.ToggleActivationAsync(id, ct);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to toggle activation.";
            }
            else
            {
                TempData["SuccessMessage"] = "Plan activation toggled successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
