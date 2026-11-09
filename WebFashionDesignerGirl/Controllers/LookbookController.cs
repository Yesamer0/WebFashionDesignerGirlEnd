using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFashionDesignerGirl.Models;
using WebFashionDesignerGirl.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebFashionDesignerGirl.Controllers
{
    public class LookbookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LookbookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- GİRİŞ: LOOKBOOK GALERİSİ ---
        // Uygulama açıldığında veya Galeriye tıklandığında tüm onaylı kombinleri listeler.
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var looks = await _context.Lookbooks
                .Include(l => l.AppUser)
                .Include(l => l.Likes)
                .Where(l => l.IsApproved)
                .Select(l => new LookbookViewModel // ViewModel isminin doğru olduğundan emin ol
                {
                    Id = l.Id,
                    LookName = l.LookName,
                    Description = l.Description,
                    Category = l.Category,
                    ImageUrl = l.ImageUrl,
                    Points = l.Points,
                    OwnerName = l.AppUser != null ? l.AppUser.UserName : "Moda Sever",
                    Likes = l.Likes.Count,
                    IsLikedByMe = userId.HasValue && l.Likes.Any(like => like.AppUserId == userId.Value)
                })
                .ToListAsync();

            // Eğer View dosyanın adı "Gallery.cshtml" ise burayı "Gallery" yap
            return View("Gallery", looks);
        }

        // --- BEĞENİ SİSTEMİ (TOGGLE LIKE) ---
        // Kullanıcı kalp ikonuna bastığında çalışır; beğeni yoksa ekler, varsa kaldırır.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike([FromBody] LookbookLikeRequest dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Giriş yapmalısın! 🎀" });

            // 1. Lookbook kaydını tüm özellikleriyle ve beğenileriyle çekiyoruz
            var look = await _context.Lookbooks
                .Include(l => l.Likes)
                .FirstOrDefaultAsync(l => l.Id == dto.LookbookId);

            if (look == null) return Json(new { success = false, message = "Kombin bulunamadı." });

            // 2. Bu kullanıcının daha önce bu kombine beğenisi var mı?
            var existingLike = await _context.LookbookLikes
                .FirstOrDefaultAsync(l => l.LookbookId == dto.LookbookId && l.AppUserId == userId.Value);

            bool isLiked;
            if (existingLike != null)
            {
                // Beğeni zaten varsa: Kaldır ve Puanı 10 düşür.
                _context.LookbookLikes.Remove(existingLike);
                look.Points = Math.Max(0, look.Points - 10);
                isLiked = false;
            }
            else
            {
                // Beğeni yoksa: Yeni beğeni ekle ve Puanı 10 artır.
                _context.LookbookLikes.Add(new LookbookLike
                {
                    LookbookId = dto.LookbookId,
                    AppUserId = userId.Value
                });
                look.Points += 10;
                isLiked = true;
            }     

            await _context.SaveChangesAsync();

            // 3. Ön yüze (JavaScript'e) güncel verileri döndürüyoruz
            return Json(new
            {
                success = true,
                liked = isLiked,
                likeCount = look.Likes.Count,
                lookName = look.LookName,
                description = look.Description,
                category = look.Category,
                points = look.Points,
                selectedItems = look.SelectedItems
            });
        }

        // --- YORUMLARI GETİR ---
        // Belirli bir kombin tıklandığında ona ait yorumları tarih sırasına göre getirir.
        [HttpGet]
        public async Task<IActionResult> GetComments(int lookbookId)
        {
            var comments = await _context.LookbookComments
                .Include(c => c.AppUser) // Yorumu yapan kişinin adını almak için
                .Where(c => c.LookbookId == lookbookId)
                .OrderByDescending(c => c.CreatedAt) // En son yorum en üstte
                .Select(c => new {
                    username = c.AppUser.UserName,
                    text = c.Text,
                    date = c.CreatedAt.ToString("dd.MM.yyyy HH:mm")
                }).ToListAsync();

            return Json(comments);
        }

        // --- YORUM EKLE ---
        // Kullanıcı yorum yazıp gönderdiğinde çalışır.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([FromBody] CommentRequest dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Yorum yapmak için giriş yapmalısın! 🎀" });

            // Boş yorum kontrolü
            if (string.IsNullOrWhiteSpace(dto.Text))
                return Json(new { success = false, message = "Yorum boş olamaz." });

            var comment = new LookbookComment
            {
                LookbookId = dto.LookbookId,
                AppUserId = userId.Value,
                Text = dto.Text.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.LookbookComments.Add(comment);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

    } // Controller Sonu

    // --- VERİ TAŞIMA MODELLERİ (DTO) ---

    // Beğeni isteği için gereken veri yapısı
    public class LookbookLikeRequest
    {
        public int LookbookId { get; set; }
    }

    // Yorum isteği için gereken veri yapısı
    public class CommentRequest
    {
        public int LookbookId { get; set; }
        public string Text { get; set; }
    }
}