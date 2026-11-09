using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFashionDesignerGirl.Models
{
    public class LookbookLike
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LookbookId { get; set; }

        [Required]
        public int AppUserId { get; set; }

        // Beğeni tarihi - Senin CreatedAt mantığınla uyumlu
        public DateTime LikedAt { get; set; } = DateTime.Now;

        [ForeignKey("LookbookId")]
        public virtual Lookbook Lookbook { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; }
    }
}