using CrudApp1.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp1.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRoleView)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole { Name = createRoleView.RoleName };

                //create Role
                IdentityResult res = await roleManager.CreateAsync(role);
                if (res.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                //add errors to modelstate
                foreach(IdentityError error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }


            }
            return View(createRoleView);
        }
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }

    }
}
