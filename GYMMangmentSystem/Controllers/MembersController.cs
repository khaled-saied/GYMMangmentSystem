using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GYMMangmentSystem.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _service;

        public MembersController(IMemberService service)
        {
            this._service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _service.GetAllMembersAsync(ct);
            return View(members);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(nameof(Create), model);

            var result = await _service.CreateMemberAsync(model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = "Failed to create member.";

            return RedirectToAction(nameof(Index));
        }

        //Datails
        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _service.GetMemberDetailsByIdAsync(id, ct);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await _service.GetMemberHealthRecordAsync(id, ct);
            if(result is null)
            {
                TempData["ErrorMessage"] = "Health Record Is not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }

        #region Edit
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _service.GetMemberToUpdateAsync(id, ct);
            if(member == null)
            {
                TempData["ErrorMessage"] = "Member Is not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result= await _service.UpdateMemberDetailsAsync(id, model, ct);

            if (result)
                TempData["SuccessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Update Member";

            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}
