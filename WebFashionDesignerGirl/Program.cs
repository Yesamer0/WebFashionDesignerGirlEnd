using Microsoft.EntityFrameworkCore;
using WebFashionDesignerGirl.Data;
using WebFashionDesignerGirl.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//  OTOMATİK KIYAFet SEED — Sadece bir kez çalışır

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var wwwroot = app.Environment.WebRootPath;

    // Klasör → Kategori eşleştirmesi
    var folders = new Dictionary<string, (string MainCat, string SubCat)>
    {
        { "basicust",  ("Üst Giyim", "Temel Üst") },
        { "basitalt",  ("Alt Giyim", "Temel Alt")  },
        // İleride yeni klasör eklersen buraya yaz:
        // { "disGiyim", ("Dış Giyim", "Ceket") },
        // { "aksesuar", ("Aksesuar",  "Takı")  },
    };

    foreach (var folder in folders)
    {
        var folderPath = Path.Combine(wwwroot, "images", "gardirop", folder.Key);
        if (!Directory.Exists(folderPath)) continue;

        var extensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var files = Directory.GetFiles(folderPath)
            .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
            .ToList();

        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var imageUrl = $"/images/gardirop/{folder.Key}/{Path.GetFileName(file)}";

            // Aynı URL zaten varsa tekrar ekleme
            var alreadyExists = context.ClothingItems.Any(c => c.ImageUrl == imageUrl);
            if (alreadyExists) continue;

            // Dosya adından güzel bir isim üret
            // Örn: "beyaz-tisort" → "Beyaz Tisort"
            var niceName = System.Globalization.CultureInfo.CurrentCulture
                .TextInfo.ToTitleCase(fileName.Replace("-", " ").Replace("_", " "));

            context.ClothingItems.Add(new ClothingItem
            {
                Name = niceName,
                ImageUrl = imageUrl,
                MainCategory = folder.Value.MainCat,
                SubCategory = folder.Value.SubCat,
                Color = "Çeşitli",
                Season = "Her Mevsim",
                PointValue = 10
            });
        }
    }

    context.SaveChanges();
}

app.Run();