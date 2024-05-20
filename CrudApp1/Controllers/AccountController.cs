using CrudApp1.Models;
using CrudApp1.Models.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Cities = new List<string>() { "Karachi", "Lahore", "Islamabad" };

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            //check model state
            if (ModelState.IsValid)
            {
                //create user
                var user = new ApplicationUser { UserName = registerViewModel.Email, Email = registerViewModel.Email, City = registerViewModel.City };
                var res = await userManager.CreateAsync(user, registerViewModel.Password);

                //sign in user if user creation succeed
                if (res.Succeeded)
                {
                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            LoginViewModel loginView = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(loginView);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl)
        {
            //check model state
            if (ModelState.IsValid)
            {

                //sign in user
                var res = await signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, false);

                if (res.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
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
            var user = await userManager.FindByEmailAsync(email);
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

        //Access denied page
        public IActionResult AccessDenied()
        {
            return View();
        }


        //External login
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnurl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnurl });

            var props = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, props);
        }

        //External login returnUrl
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl=null, string remoteError= null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginView = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            //if errror occurs add it to the model state
            if (remoteError!= null)
            {
                ModelState.AddModelError("", $"Error from External Provider : {remoteError}");
                return View("Login", loginView);
            }

            //get external provider info. if null then add to model state error.
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", $"Error getting external login information");
                return View("Login", loginView);
            }

            // sign in using third party provider
            var signinRes = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signinRes.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            // to make ExternalLoginSignInAsync there must be an entry in aspnetuserslogin table
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if(email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);

                    //if user is null then we have to create a user
                    if(user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            City="Karachi"
                        };
                        await userManager.CreateAsync(user);
                    }

                    //add to userlogins table
                    await userManager.AddLoginAsync(user, info);
                    //sign in
                    await signInManager.SignInAsync(user, isPersistent:false);

                    return LocalRedirect(returnUrl);
                }
            }

            ViewBag.ErrorMessage = $"Email Not Recieved from {info.LoginProvider}";
            return View("Error");
        }



    }
}
