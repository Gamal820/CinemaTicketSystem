using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Success()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var cartItems = _context.Carts.Where(c => c.ApplicationUserId == user.Id);
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }

            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
