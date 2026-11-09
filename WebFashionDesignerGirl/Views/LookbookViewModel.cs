using System;

namespace WebFashionDesignerGirl.Models
{
    public class LookbookViewModel
    {
        // Temel Bilgiler (Senin Lookbook modelindeki alanlar)
        public int Id { get; set; }
        public string LookName { get; set; } = "";     // LookName özelliği
        public string? Description { get; set; }        // Description özelliği
        public string? Category { get; set; }           // Category özelliği
        public string? ImageUrl { get; set; }
        public int Points { get; set; }                 // Points özelliği
        public string? SelectedItems { get; set; }      // SelectedItems özelliği
        public DateTime CreatedAt { get; set; }

        // Görselleştirme ve Beğeni İçin Ekstra Bilgiler
        public int Likes { get; set; }                  // Toplam beğeni sayısı
        public bool IsLikedByMe { get; set; }           // Giriş yapan kişi beğenmiş mi?
        public string OwnerName { get; set; } = "";     // Kombini yapanın kullanıcı adı
    }
}