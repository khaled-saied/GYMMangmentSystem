using System.Threading.Tasks;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GYMMangmentSystem.PL.Controllers
{
    public class PlansController : Controller
    {
        private readonly IGenericRepository<Plan> planRepository;
        public PlansController(IGenericRepository<Plan> planRepository)
        {
            this.planRepository = planRepository;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await planRepository.GetAllAsync(ct: ct);
            return View(plans);
        }

        public async Task<IActionResult> Details(int id,CancellationToken ct)
        {
            var plan = await planRepository.GetByIdAsync(id,ct: ct);
            if(plan == null)
                return RedirectToAction(nameof(Index));
            return View(plan);
        }
    }
}
