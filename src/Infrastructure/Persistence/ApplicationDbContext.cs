using System.Reflection;
using Domain.ConsultationRequest;
using Domain.Hashtags;
using Domain.News;
using Domain.NewsCategories;
using Domain.NewsSections;
using Domain.PackageFittings;
using Domain.PackageMaterials;
using Domain.PackageTypes;
using Domain.ProductCategories;
using Domain.ProductPhotos;
using Domain.Products;
using Domain.ProductVariants;
using Domain.RefreshTokens;
using Domain.Roles;
using Domain.Subscriptions;
using Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<NewsCategory> NewsCategories { get; set; }
    public DbSet<NewsSection> NewsSections { get; set; }
    public DbSet<Hashtag> Hashtags { get; set; }
    public DbSet<PackageType> PackageTypes { get; set; }
    public DbSet<PackageMaterial> PackageMaterials { get; set; }
    public DbSet<PackageFitting> PackageFittings { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductPhoto> ProductPhotos { get; set; }
    public DbSet<ConsultationRequest> ConsultationRequests { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}