using System.Threading.Tasks;
using GymMangment.BLL.ViewModels.AccountViewModel;
using GymMangment.DAL.Data.Models;
using GYMMangmentSystem.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GYMMangmentSystem.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
        }

        //1=>Get -=>Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        //2=>Post Login -> Submit Form
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            //Service call to check user credentials
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email Or Password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName}Is Signed in.");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if(result.IsLockedOut)
            {
                _logger.LogWarning($"User {user.UserName}Is Locked out.");
                ModelState.AddModelError("InvalidLogin", "Your Account is Locked Out.");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email Or Password.");
                return View(model);
            }
        }

        //3=>Post ->Logout
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User Logged Out.");
            return RedirectToAction(nameof(Login));
        }
        //Get -=> Access Denied
    }
}
