using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GYMMangmentSystem.PL.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberService _service;

        public MemberController(IMemberService service)
        {
            this._service = service;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _service.GetAllMembersAsync(ct);
            return View(members);
        }
    }
}
