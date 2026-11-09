using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFashionDesignerGirl.Data;
using WebFashionDesignerGirl.Models;
using System;
using System.Linq; // ÖNEMLİ: Select ve Any işlemleri için bu şart!
using System.Collections.Generic;

namespace WebFashionDesignerGirl.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ANA SAYFA ---
        public IActionResult Index()
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            // Giriş yapmamış kişiler anasayfayı görebilsin
            if (string.IsNullOrEmpty(userIdString))
            {
                return View(null);
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                return View(null);
            }

            var user = _context.AppUsers
                .Include(u => u.UserWardrobes)
                .ThenInclude(uw => uw.ClothingItem)
                .FirstOrDefault(u => u.Id == userId);

            return View(user);
        }

        // --- OYLA (RATE) SAYFASI ---
        public IActionResult Rate()
        {
            try
            {
                // 1. ADIM: Önce tüm Lookbook ID'lerini veritabanından çekiyoruz.
                // Eğer burada hata alıyorsan, veritabanında Lookbooks tablosu henüz oluşmamış olabilir.
                var lookbookIds = _context.Lookbooks.Select(l => l.Id).ToList();

                // 2. ADIM: Liste boş mu kontrol et.
                if (lookbookIds == null || !lookbookIds.Any())
                {
                    // Eğer hiç kombin yoksa hata vermemesi için boş bir model gönderiyoruz.
                    return View(null);
                }

                // 3. ADIM: Rastgele bir ID seç.
                Random rnd = new Random();
                int randomIndex = rnd.Next(0, lookbookIds.Count);
                int randomId = lookbookIds[randomIndex];

                // 4. ADIM: Seçilen ID'ye göre Lookbook ve onu tasarlayan AppUser bilgisini getir.
                // KRİTİK: Include(l => l.AppUser) çalışması için Lookbook modelinde 'AppUser' tanımı olmalı.
                var randomLookbook = _context.Lookbooks
                    .Include(l => l.AppUser)
                    .FirstOrDefault(l => l.Id == randomId);

                return View(randomLookbook);
            }
            catch (Exception ex)
            {
                // Eğer veritabanı kaynaklı bir hata olursa uygulamayı çökertme, hatayı ekrana bas.
                ViewBag.Error = "Bir hata oluştu: " + ex.Message;
                return View(null);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}