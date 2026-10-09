using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.PlanViewModels;
using GymMangment.BLL.ViewModels.TrainerViewModels;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYMMangmentSystem.PL.Controllers
{
    [Authorize]
    public class PlansController : Controller
    {
        private readonly IPlanService _service;

        public PlansController(IPlanService service)
        {
            this._service = service;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await _service.GetAllPlansAsync(ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return View(Enumerable.Empty<TrainerViewModel>());
            }
            return View(result.value);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _service.GetPlanByIdAsync(id, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.value);
        }

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _service.GetPlanToUpdateAsync(id, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _service.UpdatePlanAsync(id, model, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
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
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
            }
            else
            {
                TempData["SuccessMessage"] = "Plan activation toggled successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
