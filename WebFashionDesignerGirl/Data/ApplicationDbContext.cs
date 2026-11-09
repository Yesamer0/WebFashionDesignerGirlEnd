using Microsoft.EntityFrameworkCore;
using WebFashionDesignerGirl.Models;

namespace WebFashionDesignerGirl.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClothingItem> ClothingItems { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserWardrobe> UserWardrobes { get; set; }
        public DbSet<Lookbook> Lookbooks { get; set; }
        public DbSet<PointsLog> PointsLogs { get; set; }
        public DbSet<LookbookLike> LookbookLikes { get; set; }
        public DbSet<LookbookComment> LookbookComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Lookbook → AppUser ilişkisi
            modelBuilder.Entity<Lookbook>()
                .HasOne(l => l.AppUser)
              .WithMany(u => u.Lookbooks)
              .HasForeignKey(l => l.AppUserId)
              .OnDelete(DeleteBehavior.Restrict);


            

            // LookbookLike → Lookbook ilişkisi
            modelBuilder.Entity<LookbookLike>()
                .HasOne(l => l.Lookbook)
                .WithMany(lb => lb.Likes)
                .HasForeignKey(l => l.LookbookId)
                .OnDelete(DeleteBehavior.Cascade);

            // LookbookLike → AppUser ilişkisi
            modelBuilder.Entity<LookbookLike>()
                .HasOne(l => l.AppUser)
                .WithMany()
                .HasForeignKey(l => l.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // LookbookComment → Lookbook ilişkisi
            modelBuilder.Entity<LookbookComment>()
                .HasOne(c => c.Lookbook)
                .WithMany(lb => lb.Comments)
                .HasForeignKey(c => c.LookbookId)
                .OnDelete(DeleteBehavior.Cascade);

            // LookbookComment → AppUser ilişkisi
            modelBuilder.Entity<LookbookComment>()
                .HasOne(c => c.AppUser)
                .WithMany()
                .HasForeignKey(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}