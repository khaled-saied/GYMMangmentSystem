using System.Threading.Tasks;
using GymMangment.BLL.Services.Attachment;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.MemberViewModels;
using GymMangment.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GYMMangmentSystem.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _service;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService service,IAttachmentService attachmentService)
        {
            this._service = service;
            this._attachmentService = attachmentService;
        }

        #region Get Member Photo
        [HttpGet]
        public async Task<ActionResult> Picture(int id)
        {
            var member =await _service.GetMemberDetailsByIdAsync(id);
            if(member == null || string.IsNullOrWhiteSpace(member.value!.Photo))
                return NotFound();
            
            var result = _attachmentService.GetFile(member.value.Photo, "MembersPhoto");
            if (result == null || !result.success)
                return NotFound();


            return File(result.value.stream, result.value.ContentType);
        }
        #endregion


        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await _service.GetAllMembersAsync(ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return View(Enumerable.Empty<TrainerViewModel>());
            }
            return View(result.value);
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
            if (result.success)
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = result.error;

            return RedirectToAction(nameof(Index));
        }

        //Datails
        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var result = await _service.GetMemberDetailsByIdAsync(id, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.value);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await _service.GetMemberHealthRecordAsync(id, ct);
            if(!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.value);
        }

        #region Edit
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var result = await _service.GetMemberToUpdateAsync(id, ct);
            if(!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.value);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result= await _service.UpdateMemberDetailsAsync(id, model, ct);

            if (result.success)
                TempData["SuccessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = result.error;

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete Member
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _service.GetMemberDetailsByIdAsync(id, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.value);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id, CancellationToken ct)
        {
            var result = await _service.DeleteMemberAsync(id, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Member deleted successfully.";
            else
                TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }


        #endregion

    }
}
