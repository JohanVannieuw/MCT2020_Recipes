using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServices.Data;
using IdentityServices.Models;
using IdentityServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;  //customised !
        private readonly RoleManager<Role> _roleManager;   //customised !
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<User> userManager, RoleManager<Role> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this._context = context; //alternatief op de usermanager
        }

        // GET: Admin
        public ActionResult IndexUsers(string role)
        {
            ViewBag.Roles = _roleManager.Roles.OrderBy(r => r.Name); //voor menu opbouw

            var users = _userManager.Users.AsNoTracking<User>();
            if (role == null)
            {
                users = users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .OrderBy(u => u.UserName);
            }
            else
            {
                //filter op een gelinkte tabel met LINQ via .Any(r=> ... )
                //filter op een gelinkte tabel met LINQ via .Any(r=> ... )
                users = users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(r => r.Role.Name == role))
                .OrderBy(u => u.UserName);
            };
            return View(users);
        }


        public ActionResult IndexRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        /*----  ROLE MANAGEMENT --------------------------*/

        [HttpGet]
        public IActionResult CreateRole() => View();

        [HttpPost]
        public async Task<IActionResult> CreateRole(AddRole_VM addRoleVM)
        {

            if (!ModelState.IsValid) return View(addRoleVM);

            var role = new Role
            {
                Name = addRoleVM.RoleName
            };

            IdentityResult result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("IndexRoles", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(addRoleVM);
        }


        public async Task<IActionResult> EditRole(string id)
        {

            if (id == null)
            {
                return BadRequest(); //400
            }
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ModelState.AddModelError("", $"Role with id {id} is not found.");
                return NotFound();//404
            }
            return View("EditRole", role);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, Role role)
        {

            if (id == null)
            {
                return BadRequest(); //400
            }
            var roleFound = await _roleManager.FindByIdAsync(id);
            _context.Entry<Role>(roleFound).State = EntityState.Detached; //MUST

            if (roleFound == null)
            {
                ModelState.AddModelError("", $"Role with id {id} is not found.");
                return NotFound();//404
            }

            if (roleFound.Id != role.Id)
            {
                return BadRequest();
            }


            _context.Entry<Role>(role).State = EntityState.Modified; //MUST
            var result = _roleManager.UpdateAsync(role).Result;


            if (result.Succeeded)
            {
                return RedirectToAction(nameof(IndexRoles));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(role);

        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            Role role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("IndexRoles", _roleManager.Roles);
                ModelState.AddModelError("", "Something went wrong while deleting this role.");
            }
            else
            {
                ModelState.AddModelError("", "This role can't be found.");
            }
            return View("IndexRoles", _roleManager.Roles);
        }

        [HttpGet]
        public async Task<IActionResult> AddRoleToUser(string userId, string roleId)
        {
            User user = new User(); // vaak ApplicationUser

            if (!String.IsNullOrEmpty(userId))
            {
                user = await _userManager.FindByIdAsync(userId);
            }

            Role role = new Role();
            if (!String.IsNullOrEmpty(roleId))
            {
                role = await _roleManager.FindByIdAsync(roleId);
            }

            if (role == null && user == null)
                return RedirectToAction("IndexRoles", _roleManager.Roles);

            //Reeds toegekende rollen
            var assignRolesToUserVM = new RolesForUser_VM()
            {
                AssignedRoles = await _userManager.GetRolesAsync(user),
                UnAssignedRoles = new List<string>(),
                User = user,
                UserId = userId
            };

            //Nog niet toegekende rollen
            foreach (var identityRole in _roleManager.Roles)
            {
                if (!await _userManager.IsInRoleAsync(user, identityRole.Name))
                {
                    assignRolesToUserVM.UnAssignedRoles.Add(identityRole.Name);
                }
            }


            //Toon users in deze rol
            //foreach (var identityUser in _userManager.Users)
            //{
            //    if (!await _userManager.IsInRoleAsync(identityUser, role.Name))
            //    {
            //        addUserToRoleViewModel.Users.Add(user);
            //    }
            //}

            return View(assignRolesToUserVM);
        }
        [HttpPost]
        public async Task<IActionResult> AddUserToRole(RolesForUser_VM rolesForUserVM)
        {
            var user = await _userManager.FindByIdAsync(rolesForUserVM.UserId);
            var role = await _roleManager.FindByNameAsync(rolesForUserVM.RoleId);

            var result = await _userManager.AddToRoleAsync(user, role.NormalizedName);

            if (result.Succeeded)
            {
                return RedirectToAction("IndexRoles", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(rolesForUserVM);
        }

        //---------------------------------------------------------------------------

        //USER MANAGEMENT -----------------------------------------------------------

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            User user;

            if (!String.IsNullOrEmpty(id))
            {
                user = await _userManager.FindByIdAsync(id);
            }
            else
            {
                return BadRequest();
            }

            if (user == null)
            {
                ModelState.AddModelError("", $"User with id {id} not found");
                throw new Exception("Not found.");
            }

            return View("DetailsUser", user);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(IndexUsers));
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {

            if (id == null)
            {
                return BadRequest(); //400
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ModelState.AddModelError("", $"User withid {id} is not found.");
                return NotFound();//404
            }
            return View("EditUser", user);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, IFormCollection collection, User user)
        {
            //TODO: create_edit_model voor user aanmaken (data in view verhinderen)
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(user);
                }

                if (id == null) { return BadRequest(); }
                //extra controle op het id (optioneel)--------------------------

                User userFound = await _userManager.FindByIdAsync(id);
                _context.Entry<User>(userFound).State = EntityState.Detached; //MUST

                if (userFound == null)
                {
                    ModelState.AddModelError("", $"User with id {id} not found");
                    throw new Exception("Not found.");
                }
                if (userFound.Id != id)
                {
                    return BadRequest();
                }

                _context.Entry<User>(user).State = EntityState.Modified; //MUST
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(IndexUsers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(user);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
                return View(user);
            }
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> DeleteUser(string id)
        {
            User userFound = await _userManager.FindByIdAsync(id);
            if (userFound == null)
            {
                return NotFound(new { message = "User unkown." });
            }


            try
            {

                await _userManager.DeleteAsync(userFound);

            }
            catch
            {
                //Customised gebruikers error
                return RedirectToAction("HandleErrorCode", "Error", new
                {
                    statusCode = 400,
                    errorMessage = $"Deleting user'{userFound.UserName}' is mislukt."
                });

            }

            return RedirectToAction(nameof(IndexUsers));
        }

        //// POST: Admin/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteUser(string id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(IndexUsers));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}