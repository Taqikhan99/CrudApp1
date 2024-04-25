using CrudApp1.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            //check model state
            if (ModelState.IsValid)
            {
                //create user
                var user = new IdentityUser { UserName = registerViewModel.Email, Email = registerViewModel.Email };
                var res = await userManager.CreateAsync(user, registerViewModel.Password);
                
                //sign in user if user creation succeed
                if (res.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false); //we dont want peramant cookie so false.
                    return RedirectToAction("Index", "Home");

                }
                //add errors to modelstate
                foreach (var error in res.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(registerViewModel);
        }
    }
}
