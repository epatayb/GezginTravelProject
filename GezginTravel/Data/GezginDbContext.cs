using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GezginTravel.Models.Entities;
using GezginTravel.Models.Identity;

namespace GezginTravel.Data
{
    public class GezginDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public GezginDbContext(DbContextOptions<GezginDbContext> options)
            : base(options)
        {
        }

        // DbSet'ler
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogImage> BlogImages { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<BlogLike> BlogLikes { get; set; }
        public DbSet<BlogSave> BlogSaves { get; set; }
        public DbSet<BlogView> BlogViews { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<UserFollow> UserFollows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MANY TO MANY KEYS
            modelBuilder.Entity<BlogTag>()
                .HasKey(x => new { x.BlogId, x.TagId });

            modelBuilder.Entity<BlogCategory>()
                .HasKey(x => new { x.BlogId, x.CategoryId });

            // UNIQUE LIKE
            modelBuilder.Entity<BlogLike>()
                .HasOne(x => x.Blog)
                .WithMany()
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            // UNIQUE SAVE
            modelBuilder.Entity<BlogSave>()
                .HasOne(x => x.Blog)
                .WithMany()
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            // UNIQUE VIEW 
            modelBuilder.Entity<BlogView>()
                .HasOne(x => x.Blog)
                .WithMany()
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            // FOLLOW UNIQUE
            modelBuilder.Entity<UserFollow>()
                .HasOne(x => x.Follower)
                .WithMany(x => x.Following)
                .HasForeignKey(x => x.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasOne(x => x.Following)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasIndex(x => new { x.FollowerId, x.FollowingId })
                .IsUnique();

            // BLOG AUTHOR RELATION
            modelBuilder.Entity<Blog>()
                .HasOne(x => x.Author)
                .WithMany(x => x.Blogs)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Blog>()
                .Property(x => x.TrendScore)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BlogComment>()
                .HasOne(x => x.Blog)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogComment>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BlogComment>()
                .HasOne(x => x.ParentComment)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppUser>()
                .Property(x => x.PopularityScore)
                .HasColumnType("decimal(18,2)");

            // SOFT DELETE FILTER
            modelBuilder.Entity<Blog>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<BlogCategory>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<BlogComment>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<BlogImage>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<BlogLike>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<BlogSave>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<BlogTag>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<BlogView>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<AppUser>()
                .HasQueryFilter(x => !x.IsDeleted);
        }

        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void ApplyAuditInformation()
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                // 1.Eğer nesne BaseEntity'den türeyen bir tabloysa (Blog, Comment vs.)
                if (entry.Entity is BaseEntity baseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        baseEntity.CreatedDate = DateTime.Now;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        baseEntity.UpdatedDate = DateTime.Now;
                    }
                    if (entry.State == EntityState.Deleted)
                    {
                        baseEntity.IsDeleted = true;
                        baseEntity.DeletedDate = DateTime.Now;
                        entry.State = EntityState.Modified;
                    }
                }
                // 2. Eğer nesne AppUser (Kullanıcı) ise ve silinmeye çalışılıyorsa
                else if (entry.Entity is AppUser user)
                {
                    if (entry.State == EntityState.Deleted)
                    {
                        user.IsDeleted = true;
                        user.DeletedDate = DateTime.Now;
                        entry.State = EntityState.Modified;
                    }
                }
            }
        }
    }
}