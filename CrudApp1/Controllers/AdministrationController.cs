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

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleid)
        {
            var role = await roleManager.FindByIdAsync(roleid);

            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id: {roleid} is not found";
                return View("Error");
            }

            ViewBag.roleid=roleid;

            var model = new List<UserRolesViewModel>();

            foreach(var user in userManager.Users.ToList())
            {
                var userRoleVm = new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,

                };
                //check if in role then set isSelected to true else false
                if(await userManager.IsInRoleAsync(user, roleid))
                {
                    userRoleVm.IsSelected= true;
                }
                else
                {
                    userRoleVm.IsSelected= false;
                }

                model.Add(userRoleVm);

            }
            return View(model);
        }

        //post method for edituserinrole
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRolesViewModel> model,string roleid)
        {
            var role = await roleManager.FindByIdAsync(roleid);
            //if role is null then redirect to error page
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id: {roleid} is not found";
                return View("Error");
            }

            //for each user in model
            for (int i = 0; i < model.Count; i++)
            {
                //get user
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult res = null;

                //add to role if checked andnot already assigned
                if (model[i].IsSelected && !await userManager.IsInRoleAsync(user, role.Name))
                {
                    res= await userManager.AddToRoleAsync(user,role.Name);
                }
                else if(!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    res= await userManager.RemoveFromRoleAsync(user,role.Name);
                }
                else
                {
                    continue;
                }

                //if res succeed
                if (res.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { id = roleid });
                }

            }

            return RedirectToAction("EditRole", new { id = roleid });
            
        }
    }
}
