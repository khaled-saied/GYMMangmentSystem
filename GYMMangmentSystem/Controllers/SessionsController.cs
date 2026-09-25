using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GYMMangmentSystem.PL.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _service;

        public SessionsController(ISessionService service)
        {
            this._service = service;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _service.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        #region Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownListsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownListsAsync();
                return View(model);
            }
            var result = await _service.CreateSessionAsync(model, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Session created successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Failed to create session.";
            await PopulateDropDownListsAsync();
            return View(model);
        }

        private async Task PopulateDropDownListsAsync()
        {
            ViewBag.Trainers = new SelectList(await _service.GetTrainersForDropDownAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _service.GetCategoriesForDropDownAsync(), "Id", "CategoryName");
        }

        #endregion
    }
}
