using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFashionDesignerGirl.Models
{
    public class LookbookComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LookbookId { get; set; } // Hangi kombine yorum yapıldı?

        [Required]
        public int AppUserId { get; set; } // Yorumu kim yaptı?

        [Required(ErrorMessage = "Yorum boş bırakılamaz! 🎀")]
        [StringLength(500)]
        public string Text { get; set; } // Yorum metni

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Senin modelindeki tarih yapısı

        // İlişkiler
        [ForeignKey("LookbookId")]
        public virtual Lookbook Lookbook { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; }
    }
}