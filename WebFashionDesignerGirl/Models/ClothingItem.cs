namespace WebFashionDesignerGirl.Models
{
    public class ClothingItem
    {
        public int Id { get; set; } // Her kıyafetin benzersiz numarası

        public string Name { get; set; } // Örn: "Gül Kurusu Blazer Ceket"

        public string ImageUrl { get; set; } // Fotoğrafın internet adresi veya dosya yolu

        // --- SENİN LİSTENE GÖRE EKLENENLER ---

        // Ana Kategori: Üst Giyim, Alt Giyim, Aksesuar vb.
        public string MainCategory { get; set; }

        // Alt Kategori: Tişört, Jean, Blazer, Küpe vb.
        public string SubCategory { get; set; }

        // Renk: Sakura, Dark Chocolate gibi filtreleme için
        public string Color { get; set; }

        // Mevsim: Yazlık, Kışlık
        public string Season { get; set; }

        public int PointValue { get; set; } // Bu parçayı kullanınca kazanılan stil puanı
    }
}