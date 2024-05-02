using Dannys.Interceptors;
using Dannys.Models;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Data;

public class AppDbContext : DbContext
{

    private readonly BaseEntityInterceptor _interceptor;


    public AppDbContext(DbContextOptions<AppDbContext> options, BaseEntityInterceptor interceptor) : base(options)
    {
        _interceptor = interceptor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted);


    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(_interceptor);
    }


    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<ProductImg> ProductImgs { get; set; } = null!;
    public DbSet<Slider> Sliders { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Basketitem> Basketitems { get; set; } = null!;
    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<BlogTopic> BlogTopics { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Table> Tables { get; set; } = null!;
}

