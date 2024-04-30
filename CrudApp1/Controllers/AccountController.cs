using CrudApp1.Models;
using CrudApp1.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudApp1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Cities = new List<string>() { "Karachi", "Lahore", "Islamabad" };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            //check model state
            if (ModelState.IsValid)
            {
                //create user
                var user = new ApplicationUser { UserName = registerViewModel.Email, Email = registerViewModel.Email,City=registerViewModel.City };
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


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel,string returnUrl)
        {
            //check model state
            if (ModelState.IsValid)
            {

                //sign in user
                var res = await signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, false);

                if(res.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return RedirectToAction(returnUrl);
                       //or use return LocalRedirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                //add error to modelstate
                ModelState.AddModelError(string.Empty, "Login Failed, Invalid Username or password");

            }
            return View(loginViewModel);
        }

        //remote validation for email
        //or use [AcceptVerbs("GET","POST")
        [HttpGet]
        public async Task<IActionResult> IsEmailAlreadyInUse(string email)
        {
            //get the user from db with email passed
            var user = userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            return Json($"Email {email} is already in use. Try some like {await GenerateUniqueUsernamesAsync(email)}");
        }
        public async Task<string> GenerateUniqueUsernamesAsync(string baseUsername, int count = 1)
        {
            var suggestions = new List<string>();
            string suggestion;
            while (suggestions.Count < count)
            {
                
                suggestion = $"{baseUsername}{new Random().Next(100, 999)}";
                
                suggestions.Add(suggestion);
            }
            return string.Join(", ", suggestions);
        }
    

    //signout
    public async Task<IActionResult> Signout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
