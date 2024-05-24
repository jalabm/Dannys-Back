using System.Reflection;
using Dannys.Configurations;
using Dannys.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Data;

public class AppDbContext : IdentityDbContext
{

    private readonly BaseEntityInterceptor _interceptor;


    public AppDbContext(DbContextOptions<AppDbContext> options, BaseEntityInterceptor interceptor) : base(options)
    {
        _interceptor = interceptor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ProductConfiguration());
        base.OnModelCreating(builder);

        AddedQueryFilter(builder);
        AddedRoles(builder);
        AddedAdminUser(builder);
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
    public DbSet<Coupon> Coupons { get; set; } = null!;




    private static void AddedQueryFilter(ModelBuilder builder)
    {
        builder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Category>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Author>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Topic>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Blog>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Coupon>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Basketitem>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Comment>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Order>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Reservation>().HasQueryFilter(x => !x.IsDeleted);
    }

    private static void AddedAdminUser(ModelBuilder builder)
    {
        AppUser appUser = new()
        {
            Id = "1",
            Name = "Admin",
            Surname = "Admin",
            UserName = "admin",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            NormalizedUserName = "ADMIN",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAEAACcQAAAAEIFavcXSkUaaUvyIby+VzzxiM4Tm/ULRmnIUQIQ3WZNNh7oA4E/GwvuXma3yJ24sew==",
            SecurityStamp = "SPU6GFMFF6OXBLM2PSPAAUU375TGRCUO"
        };


        builder.Entity<AppUser>().HasData(appUser);


        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>() { RoleId = "1", UserId = "1" });
    }

    private static void AddedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole() { Name = "Admin", NormalizedName = "ADMIN", Id = "1" },
            new IdentityRole() { Name = "Member", NormalizedName = "MEMBER", Id = "2" },
            new IdentityRole() { Name = "Moderator", NormalizedName = "MODERATOR", Id = "3" }

            );
    }
}

