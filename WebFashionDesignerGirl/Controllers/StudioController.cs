using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFashionDesignerGirl.Data;
using WebFashionDesignerGirl.Models;

namespace WebFashionDesignerGirl.Controllers
{
    public class StudioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        // GET /Studio/Index
        // ─────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var wardrobe = await _context.UserWardrobes
                .Include(w => w.ClothingItem)
                .Where(w => w.AppUserId == userId.Value)
                .OrderBy(w => w.ClothingItem.MainCategory)
                .ThenBy(w => w.ClothingItem.Name)
                .ToListAsync();

            return View("Create", wardrobe);
        }

        // ─────────────────────────────────────────────
        // POST /Studio/Create
        // ─────────────────────────────────────────────
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateOutfitDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false, message = "Giriş yapmalısın." });

            if (string.IsNullOrWhiteSpace(dto.LookName))
                return Json(new { success = false, message = "Kombin adı boş olamaz." });

            if (string.IsNullOrWhiteSpace(dto.SelectedItems))
                return Json(new { success = false, message = "En az 1 parça seçmelisin." });

            var selectedIds = dto.SelectedItems
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            var totalPoints = await _context.ClothingItems
                .Where(c => selectedIds.Contains(c.Id))
                .SumAsync(c => c.PointValue);

            try
            {
                var lookbook = new Lookbook
                {
                    AppUserId = userId.Value,
                    LookName = dto.LookName.Trim(),
                    Description = dto.Description?.Trim(),
                    Category = dto.Category?.Trim(),
                    ImageUrl = dto.ImageUrl?.Trim(),
                    SelectedItems = dto.SelectedItems,
                    Points = totalPoints,
                    IsApproved = true,
                    CreatedAt = DateTime.Now
                };

                // Navigation property'leri null bırak — EF karıştırmasın
                lookbook.AppUser = null!;
                lookbook.Likes = null!;
                lookbook.Comments = null!;

                _context.Entry(lookbook).State = EntityState.Added;

                // Kullanıcı puanını güncelle
                var user = await _context.AppUsers.FindAsync(userId.Value);
                if (user != null)
                {
                    user.StylePoints += totalPoints;
                    _context.Entry(user).Property(u => u.StylePoints).IsModified = true;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, lookbookId = lookbook.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }
    

        // ─────────────────────────────────────────────
        // POST /Studio/UploadImage  →  Base64 görseli kaydet
        // ─────────────────────────────────────────────
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UploadImage([FromBody] UploadImageDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.ImageData))
                    return Json(new { success = false });

                // Base64 prefix'i kaldır: "data:image/jpeg;base64,..."
                var base64 = dto.ImageData.Contains(',')
                    ? dto.ImageData.Split(',')[1]
                    : dto.ImageData;

                var bytes = Convert.FromBase64String(base64);

                // Kayıt klasörü: wwwroot/images/combinations/
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot", "images", "combinations");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"combo_{DateTime.Now:yyyyMMddHHmmssfff}.jpg";
                var filePath = Path.Combine(uploadsFolder, fileName);
                var imageUrl = $"/images/combinations/{fileName}";

                await System.IO.File.WriteAllBytesAsync(filePath, bytes);

                return Json(new { success = true, imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    } }

    // ── DTO ──────────────────────────────────────
    public class UploadImageDto
{
    public string ImageData { get; set; } = "";
}

public class CreateOutfitDto
{
    public string LookName { get; set; } = "";
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public string SelectedItems { get; set; } = "";
}
