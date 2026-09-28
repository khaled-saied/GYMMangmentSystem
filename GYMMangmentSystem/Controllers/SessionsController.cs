using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.SessionViewModels;
using GymMangment.DAL.Data.Models;
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
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session created successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            await PopulateDropDownListsAsync();
            return View(model);
        }

        private async Task PopulateDropDownListsAsync()
        {
            ViewBag.Trainers = new SelectList(await _service.GetTrainersForDropDownAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _service.GetCategoriesForDropDownAsync(), "Id", "CategoryName");
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult> Details(int Id,CancellationToken ct)
        {
            var result = await _service.GetSessionByIdAsync(Id, ct);

            if (result.success)
            {
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        #region Edit
        [HttpGet]
        public async Task<ActionResult> Edit(int id ,CancellationToken ct)
        {
            var session =await _service.GetSessionToUpdateAsync(id, ct);

            if(session.success)
            {
                ViewBag.Trainers = new SelectList(await _service.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(session.value);
            }
            else
            {
                TempData["ErrorMessage"] = session.error;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<ActionResult> Edit(int id,UpdateSessionViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await _service.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }

            var result =  await _service.UpdateSessionAsync(id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {

                TempData["ErrorMessage"] = result.error;
                ViewBag.Trainers = new SelectList(await _service.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }
        }

        #endregion

    }
}
