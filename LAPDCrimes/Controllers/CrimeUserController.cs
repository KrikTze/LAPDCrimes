using LAPDCrimes.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LAPDCrimes.Controllers
{
    public class CrimeUserController : Controller
    {
        private readonly UserManager<CrimeUser> _userManager;
        private readonly SignInManager<CrimeUser> _signInManager;
        public CrimeUserController(UserManager<CrimeUser> userManager, SignInManager<CrimeUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]

        public async Task<IActionResult> Register(RegisterModel user)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            if (ModelState.IsValid)
            {

                //some type of filtering here
                CrimeUser CrimeUser = new CrimeUser()
                {
                    UserName = user.username,
                    firstName = user.firstName,
                    lastName = user.lastName,
                    gender = user.gender,
                    Email = user.Email,
                };
                var result = await _userManager.CreateAsync(CrimeUser, user.password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(CrimeUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Login")]

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]

        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //find the user with the email
                var result = await _signInManager.PasswordSignInAsync(model.username, model.password, false, false);
                Console.WriteLine("Failed Society");
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            }
            return RedirectToAction("Index", "Home");
        }


    }
}
