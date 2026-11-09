using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFashionDesignerGirl.Models
{
    public class Lookbook
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string LookName { get; set; }
        public string SelectedItems { get; set; }
        public bool IsApproved { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; }

        // Bu iki satırın varlığından emin ol, hataların çoğu buradan geliyor:
        public virtual ICollection<LookbookLike> Likes { get; set; } = new List<LookbookLike>();
        public virtual ICollection<LookbookComment> Comments { get; set; } = new List<LookbookComment>();
    }
}