using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}
