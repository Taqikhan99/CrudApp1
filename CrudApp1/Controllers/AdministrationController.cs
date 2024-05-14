using CrudApp1.Models;
using CrudApp1.Models.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace CrudApp1.Controllers
{
    //lets make this controller accessable for admins only
    [Authorize(Roles = "Admin,User")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager,ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }
        public IActionResult CreateRole()
        {
            logger.LogInformation("Getting to Create Role");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRoleView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityRole role = new IdentityRole { Name = createRoleView.RoleName };

                    //create Role
                    IdentityResult res = await roleManager.CreateAsync(role);
                    if (res.Succeeded)
                    {
                        logger.LogInformation("Created New Role Successfully");
                        return RedirectToAction("GetRoles", "Administration");
                    }
                    //add errors to modelstate
                    foreach (IdentityError error in res.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }


                }
                return View(createRoleView);
            }
            catch(Exception ex)
            {
                logger.LogError("Create Role Failed. Fail Time:  "+DateTime.Now);
                ViewBag.ErrorMessage=ex.Message;
                return View("Error");
            }
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

        //Delete Role
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            try
            {
                
                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role With Id: {id} is not found";
                    return View("Error");
                }

                var res = await roleManager.DeleteAsync(role);
                if (res.Succeeded)
                {
                    logger.LogInformation("Delete Role Success. Time:  " + DateTime.Now);
                    return RedirectToAction("GetRoles");
                }
                foreach (var e in res.Errors)
                {
                    ModelState.AddModelError("", e.Description);
                }

                return View("GetRoles");
            }
            catch(DbUpdateException ex)
            {
                logger.LogError("Delete Role Failed. Fail Time:  " + DateTime.Now);
                ViewBag.ErrorMessage = @$"{role.Name} role is in use by users and cannot be deleted!  ";
                return View("Error");
            }
            catch(Exception ex)
            {
                logger.LogError("Delete Role Failed. Fail Time:  " + DateTime.Now);
                ViewBag.ErrorMessage=ex.Message;
                return View("Error");
            }
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

            var model = new List<UsersInRoleViewModel>();

            foreach(var user in userManager.Users.ToList())
            {
                var userRoleVm = new UsersInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,

                };
                //check if in role then set isSelected to true else false
                if(await userManager.IsInRoleAsync(user, role.Name))
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
        public async Task<IActionResult> EditUsersInRole(List<UsersInRoleViewModel> model,string roleid)
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

        #region ManageUsers
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsers(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User With Id: {id} is not found";
                return View("Error");
            }
            //get claims and roles for the user to populate in view model
            var roles = await userManager.GetRolesAsync(user);
            var claims= await userManager.GetClaimsAsync(user);

            //initialize view model
            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                City = user.City,
                Roles = roles.ToList(),
                Claims = claims.Select(x => x.Value).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsers(EditUserViewModel viewModel)
        {
            var user = await userManager.FindByIdAsync(viewModel.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User With Id: {viewModel.Id} is not found";
                return View("Error");
            }
            //else update the user props
            user.Email=viewModel.Email;
            user.City=viewModel.City;
            user.UserName=viewModel.UserName;
            var res = await userManager.UpdateAsync(user);

            if (res.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }
            //else add errors to  modelstate and return
            foreach(var e in res.Errors)
            {
                ModelState.AddModelError("", e.Description);
            }

            return View(viewModel);
        }


        //delete user
        
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User With Id: {id} is not found";
                return View("Error");
            }

            var res= await userManager.DeleteAsync(user);
            if (res.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }
            foreach (var e in res.Errors)
            {
                ModelState.AddModelError("", e.Description);
            }

            return View("ListUsers");
        }

        //manage user roles
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userid)
        {

            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User With Id: {userid} is not found";
                return View("Error");
            }

            ViewBag.userid=userid; 
            //create model and display all the roles on page if contain then isselected = true else false
            var model = new List<UserRolesViewModel>();

            foreach(var role in roleManager.Roles.ToList())
            {
                var userRoleVm =new UserRolesViewModel {  RoleId = role.Id, RoleName = role.Name };
                //check if in role then set isSelected to true else false
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleVm.IsSelected = true;
                }
                else
                {
                    userRoleVm.IsSelected = false;
                }
                //add to list of roles
                model.Add(userRoleVm);
            }

            return View(model);

            

        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model ,string userid)
        {

            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User With Id: {userid} is not found";
                return View("Error");
            }
            //get roles for user then remove first all roles
            var roles = await userManager.GetRolesAsync(user);
            var res = await userManager.RemoveFromRolesAsync(user, roles);

            if (!res.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove roles");
                return View(model);
            }

            //select the roles user has checked and save
            res = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!res.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove roles");
                return View(model);
            }
            return RedirectToAction("EditUsers", new { id = user.Id });

        }

        //Claims
        public async Task<IActionResult> ManageUserClaims(string userid)
        {
            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User With Id: {userid} is not found";
                return View("Error");
            }

            //get the existing claims using user manager class
            var existingClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userid
            };


            //traverse through claims in claimstore
            foreach(Claim claim in ClaimsStore.claims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                //if user has the claim set isSelected to true
                if(existingClaims.Any(x=>x.Type == claim.Type))
                {
                    userClaim.IsSelected=true;
                }
                model.Claims.Add(userClaim);

            }
            return View(model);




        }



            #endregion
        }
}
