using CinemaTicketSystem.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaTicketSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using Stripe.Checkout;

namespace CinemaTicketSystem.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
      
        public CartController(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var cartItems = await _context.Carts
                .Include(c => c.Movie)
                .Where(c => c.ApplicationUserId == user.Id)
                .ToListAsync();

            ViewBag.Total = cartItems.Sum(i => i.Price * i.Count);
            return View(cartItems);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            
            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.MovieId == movieId && c.ApplicationUserId == user.Id);

            if (existingCartItem != null)
            {
                existingCartItem.Count += 1;
            }
            else
            {
                var newCartItem = new Cart
                {
                    MovieId = movie.Id,
                    ApplicationUserId = user.Id,
                    Count = 1,
                    Price = movie.Price
                };
                _context.Carts.Add(newCartItem);
            }

            await _context.SaveChangesAsync();

            TempData["success"] = $"{movie.Name} added to your cart successfully!";
            return RedirectToAction("Index");
        }

       
        [HttpPost]
        public async Task<IActionResult> ApplyCode(string code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.Carts
                .Include(c => c.Movie)
                .Where(c => c.ApplicationUserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                ViewBag.Message = "Your cart is empty.";
                ViewBag.Total = 0;
                return View("Index", cartItems);
            }

            decimal totalBeforeDiscount = cartItems.Sum(i => i.Price * i.Count);
            decimal totalAfterDiscount = totalBeforeDiscount;

            var promo = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == code && p.IsActive && p.EndDate > DateTime.Now);

            if (promo != null)
            {
                var discount = totalBeforeDiscount * (decimal)promo.DiscountPercent / 100;
                totalAfterDiscount -= discount;

                ViewBag.Message = $"✅ Promo code '{promo.Code}' applied — {promo.DiscountPercent}% off!";
                ViewBag.Discount = discount;

                //   حفظ السعر بعد الخصم في Session
                HttpContext.Session.SetString("CartTotalAfterDiscount", totalAfterDiscount.ToString());
            }
            else
            {
                ViewBag.Message = "❌ Invalid or expired code.";
                ViewBag.Discount = 0;

                //   امسح أي خصم سابق
                HttpContext.Session.Remove("CartTotalAfterDiscount");
            }



            ViewBag.TotalBefore = totalBeforeDiscount;
            ViewBag.TotalAfter = totalAfterDiscount;

            return View("Index", cartItems);
        }


      
        public async Task<IActionResult> Increment(int id)
        {
            var cartItem = await _context.Carts.FindAsync(id);
            if (cartItem != null)
            {
                cartItem.Count++;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
 
        public async Task<IActionResult> Decrement(int id)
        {
            var cartItem = await _context.Carts.FindAsync(id);
            if (cartItem != null && cartItem.Count > 1)
            {
                cartItem.Count--;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

      
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var cartItem = await _context.Carts.FindAsync(id);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account", new { area = "Identity" });

            var cartItems = await _context.Carts
                .Include(c => c.Movie)
                .Where(c => c.ApplicationUserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index");

            //   قراءة السعر بعد الخصم من Session (إن وجد)
            decimal? discountedTotal = null;
            if (HttpContext.Session.GetString("CartTotalAfterDiscount") != null)
                discountedTotal = decimal.Parse(HttpContext.Session.GetString("CartTotalAfterDiscount"));

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel",
            };

            if (discountedTotal.HasValue)
            {
                //   لو في خصم نرسل Stripe سعر واحد بإجمالي بعد الخصم
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = "Total After Discount" },
                        UnitAmount = (long)(discountedTotal.Value * 100)
                    },
                    Quantity = 1
                });
            }
            else
            {
                //  بدون خصم
                foreach (var item in cartItems)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Movie.Name,
                                Description = item.Movie.Description
                            },
                            UnitAmount = (long)(item.Price * 100),
                        },
                        Quantity = item.Count,
                    });
                }
            }

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

    }

}

