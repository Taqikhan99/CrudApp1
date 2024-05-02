using CrudApp1.Models;
using CrudApp1.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp1.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
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
                    return RedirectToAction("GetRoles", "Administration");
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

        //Edit Role
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {

            var role = await roleManager.FindByIdAsync(id);
            
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id: {id} is not found";
                return View("Error");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
            };

            foreach(var user in userManager.Users.ToList())
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.UsersInRole.Add(user.UserName);
                }
            }

            return View(model);

        }


        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel roleViewModel)
        {
            var role = await roleManager.FindByIdAsync(roleViewModel.Id);
            //if role is null then redirect to error page
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id: {roleViewModel.Id} is not found";
                return View("Error");
            }
            //else update the role
            else
            {
                role.Name = roleViewModel.Name;
                var res= await roleManager.UpdateAsync(role);

                if (res.Succeeded)
                {
                    return RedirectToAction("GetRoles");
                }

                foreach(var error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(roleViewModel);
        }


    }
}
