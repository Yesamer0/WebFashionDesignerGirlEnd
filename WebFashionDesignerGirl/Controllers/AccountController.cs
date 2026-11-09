using Microsoft.AspNetCore.Mvc;
using WebFashionDesignerGirl.Models;
using WebFashionDesignerGirl.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace WebFashionDesignerGirl.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        public IActionResult Register() => View("Login");

        // --- PROFİL GÖRÜNTÜLEME ---
        public IActionResult Profile(int? id)
        {
            AppUser user;

            if (id.HasValue)
            {
                user = _context.AppUsers
                    .Include(u => u.UserWardrobes)
                    .ThenInclude(w => w.ClothingItem)
                    .Include(u => u.Lookbooks)
                    .FirstOrDefault(u => u.Id == id.Value);
            }
            else
            {
                var loggedInUserId = HttpContext.Session.GetInt32("UserId");
                if (loggedInUserId == null) return RedirectToAction("Login");

                user = _context.AppUsers
                    .Include(u => u.UserWardrobes)
                    .ThenInclude(w => w.ClothingItem)
                    .Include(u => u.Lookbooks)
                    .FirstOrDefault(u => u.Id == loggedInUserId);
            }

            if (user == null) return RedirectToAction("Login");

            return View("Profile", user);
        }

        // --- KAYIT OLMA ---
        [HttpPost]
        public IActionResult Register(string Name, string Surname, int Age, string ModaAdi, string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ViewBag.Error = "E-posta veya şifre boş bırakılamaz! 🎀";
                return View("Login");
            }

            var exists = _context.AppUsers.Any(u => u.ModaAdi == ModaAdi || u.Email == Email);
            if (exists)
            {
                ViewBag.Error = "Bu Moda Adı veya E-posta zaten alınmış! 🎀";
                return View("Login");
            }

            var newUser = new AppUser
            {
                UserName = Name + " " + Surname,
                ModaAdi = ModaAdi,
                Age = Age,
                Email = Email,
                Password = Password,
                StylePoints = 500,
                UserWardrobes = new List<UserWardrobe>()
            };

            Random rnd = new Random();
            var allUsts = _context.ClothingItems.Where(c => c.MainCategory == "Üst Giyim").ToList();
            var allAlts = _context.ClothingItems.Where(c => c.MainCategory == "Alt Giyim").ToList();

            if (allUsts.Any() && allAlts.Any())
            {
                var giftUsts = allUsts.OrderBy(x => rnd.Next()).Take(2).ToList();
                var giftAlts = allAlts.OrderBy(x => rnd.Next()).Take(2).ToList();
                foreach (var item in giftUsts.Concat(giftAlts))
                {
                    newUser.UserWardrobes.Add(new UserWardrobe { ClothingItemId = item.Id });
                }
            }

            _context.AppUsers.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.Id);
            HttpContext.Session.SetInt32("UserPoints", newUser.StylePoints);
            HttpContext.Session.SetString("UserEmail", newUser.Email);

            if (newUser.Email == "yesamer@fashiongirl.com")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Profile", new { id = newUser.Id });
        }

        // --- GİRİŞ YAPMA ---
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var user = _context.AppUsers
                .Include(u => u.UserWardrobes)
                .ThenInclude(w => w.ClothingItem)
                .FirstOrDefault(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetInt32("UserPoints", user.StylePoints);
                HttpContext.Session.SetString("UserEmail", user.Email);

                if (user.Email == "yesamer@fashiongirl.com")
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Profile", new { id = user.Id });
            }

            ViewBag.Error = "E-posta veya şifre hatalı! 🎀";
            return View();
        }

        // --- ÇIKIŞ YAPMA ---
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // --- PROFİL FOTOĞRAFI GÜNCELLEME ---
        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile profilePic, int userId)
        {
            if (profilePic != null && profilePic.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePic.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePic.CopyToAsync(stream);
                }

                var user = _context.AppUsers.Find(userId);
                if (user != null)
                {
                    user.ProfilePictureUrl = "/uploads/" + fileName;
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Profile", new { id = userId });
        }

        // --- KIYAFET SATIN ALMA ---
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> BuyItem([FromBody] BuyItemDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısın." });

            var user = await _context.AppUsers.FindAsync(userId.Value);
            if (user == null)
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            if (user.StylePoints < 100)
                return Json(new { success = false, message = "Yeterli puanın yok! En az 100 puana ihtiyacın var. 🎀" });

            var ownedIds = _context.UserWardrobes
                .Where(w => w.AppUserId == userId.Value)
                .Select(w => w.ClothingItemId)
                .ToList();

            var available = _context.ClothingItems
    .Where(c => c.MainCategory == dto.Category || c.MainCategory == dto.Category.Replace(" Giyim", ""))
    .ToList();


            if (!available.Any())
                return Json(new { success = false, message = "Bu kategoride yeni kıyafet kalmadı!" });

            var rnd = new Random();
            var item = available[rnd.Next(available.Count)];

            _context.UserWardrobes.Add(new UserWardrobe
            {
                AppUserId = userId.Value,
                ClothingItemId = item.Id
            });

            user.StylePoints -= 100;
            HttpContext.Session.SetInt32("UserPoints", user.StylePoints);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                itemName = item.Name,
                imageUrl = item.ImageUrl,
                remainingPoints = user.StylePoints
            });
        }
    }

    public class BuyItemDto
    {
        public string Category { get; set; } = "";
    }
}