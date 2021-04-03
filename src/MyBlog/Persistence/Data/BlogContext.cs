using Microsoft.EntityFrameworkCore;
using MyBlog.Entity;

namespace MyBlog.Persistence.Data
{
  public class BlogContext : DbContext
  {
    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {

    }

    public DbSet<BlogTable> Blogs { get; set; }
    public DbSet<TagTable> Tags { get; set; }
    public DbSet<BlobTable> Blobs { get; set; }
    public DbSet<PushNotificationSubscriptionTable> PushNotificationSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<BlogTable>()
        .HasIndex(b => b.Slug)
        .IsUnique();
    }
  }
}
