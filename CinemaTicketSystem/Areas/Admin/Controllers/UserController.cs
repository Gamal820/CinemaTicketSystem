using CinemaTicketSystem.Models;
using CinemaTicketSystem.Utitlies;
using CinemaTicketSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.SUPER_ADMIN_ROLE)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

       
        public async Task<IActionResult> Index(string? searchTerm, string? role)
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users
                    .Where(u =>
                        (u.FirstName + " " + u.LastName).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            
            if (!string.IsNullOrEmpty(role))
            {
                var usersInRole = new List<ApplicationUser>();
                foreach (var user in users)
                {
                    if (await _userManager.IsInRoleAsync(user, role))
                        usersInRole.Add(user);
                }
                users = usersInRole;
            }

            var vm = new ManageUsersVM
            {
                Users = users,
                Roles = roles,
                SearchTerm = searchTerm,
                SelectedRole = role
            };

            return View(vm);
        }
 
        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, SD.SUPER_ADMIN_ROLE))
            {
                TempData["error-notification"] = "You cannot block the Super Admin account";
                return RedirectToAction("Index");
            }

            user.LockoutEnabled = !user.LockoutEnabled;

            if (!user.LockoutEnabled)
                user.LockoutEnd = DateTime.UtcNow.AddDays(30);
            else
                user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);

            TempData["success-notification"] = $"Updated status for {user.FirstName} {user.LastName}";
            return RedirectToAction("Index");
        }

        
        [HttpGet]
        public async Task<IActionResult> ManageRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var currentRoles = await _userManager.GetRolesAsync(user);

            var vm = new ManageRoleVM
            {
                UserId = user.Id,
                UserName = $"{user.FirstName} {user.LastName}",
                AllRoles = allRoles,
                CurrentRole = currentRoles.FirstOrDefault()
            };

            return View(vm);
        }

         
        [HttpPost]
        public async Task<IActionResult> ManageRole(ManageRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.NewRole);

            TempData["success-notification"] = $"Updated role for {user.FirstName} {user.LastName}";
            return RedirectToAction("Index");
        }
    }
}
