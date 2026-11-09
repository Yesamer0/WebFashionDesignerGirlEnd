using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFashionDesignerGirl.Data;
using WebFashionDesignerGirl.Models;
using System.Linq;

namespace WebFashionDesignerGirl.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ana Admin Sayfası: Üyeleri Listeler
        
        public IActionResult Index()
        {
            var users = _context.AppUsers.ToList();

            
            return View("UserList", users);
        }

        // Üye Detay Sayfası: Hediye verme ekranı
        public IActionResult UserDetails(int id)
        {
            var user = _context.AppUsers
                .Include(u => u.UserWardrobes)
                .ThenInclude(w => w.ClothingItem)
                .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            // Tüm kıyafet kataloğunu yan tarafa koymak için çekiyoruz
            ViewBag.AllClothingItems = _context.ClothingItems.ToList();

            return View(user);
        }

        // Hediye Gönderme Butonuna Basınca Çalışacak Kod
        [HttpPost]
        public IActionResult GiftItem(int userId, int itemId)
        {
            var exists = _context.UserWardrobes.Any(w => w.AppUserId == userId && w.ClothingItemId == itemId);

            if (!exists)
            {
                _context.UserWardrobes.Add(new UserWardrobe
                {
                    AppUserId = userId,
                    ClothingItemId = itemId
                });
                _context.SaveChanges();
            }

            return RedirectToAction("UserDetails", new { id = userId });
        }
    }
}