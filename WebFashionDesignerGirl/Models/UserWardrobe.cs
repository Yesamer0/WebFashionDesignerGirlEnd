namespace WebFashionDesignerGirl.Models
{
    public class UserWardrobe
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int ClothingItemId { get; set; }

        // HATA ÇÖZÜMÜ: Bu iki satırı (navigation properties) eklemelisin
        public virtual AppUser AppUser { get; set; }
        public virtual ClothingItem ClothingItem { get; set; }
    }
}