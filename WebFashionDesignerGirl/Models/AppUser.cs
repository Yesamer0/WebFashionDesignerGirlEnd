using System.ComponentModel.DataAnnotations;

namespace WebFashionDesignerGirl.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int StylePoints { get; set; }
        public int Age { get; set; }

        [Required]
        public string ModaAdi { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string? ProfilePictureUrl { get; set; } // Fotoğrafın yolunu burada tutuyoruz

        // Kullanıcının paylaştığı kombinleri tutan koleksiyon
        public virtual ICollection<Lookbook> Lookbooks { get; set; } = new List<Lookbook>();
        public virtual ICollection<UserWardrobe> UserWardrobes { get; set; } = new List<UserWardrobe>();
    }
}