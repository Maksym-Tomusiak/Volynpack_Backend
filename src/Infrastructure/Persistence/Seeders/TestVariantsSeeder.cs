using Domain;
using Domain.ProductPhotos;
using Domain.ProductVariants;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeds 1,000 test ProductVariants across all existing Products and Materials.
/// Call ONLY in development environments.
/// </summary>
public static class TestVariantsSeeder
{
    // The 4 placeholder image paths relative to the storage root
    private static readonly string[] TestImages =
    [
        "/uploads/test/bag-1.png",
        "/uploads/test/bag-2.png",
        "/uploads/test/bag-3.png",
        "/uploads/test/bag-4.png",
    ];

    // Representative dimension pools (height × width × depth? in mm)
    private static readonly (decimal H, decimal W, decimal? D)[] Sizes =
    [
        (150, 100, null),  (200, 120, null),  (250, 150, null),
        (300, 180, null),  (350, 200, null),  (400, 250, null),
        (450, 300, null),  (500, 350, null),  (200, 120, 80),
        (250, 150, 100),   (300, 180, 120),   (350, 200, 140),
        (400, 250, 160),   (500, 300, 180),   (600, 400, 200),
        (100,  80, null),  (120,  90, null),  (180, 110, null),
        (220, 140, null),  (280, 170, null),
    ];

    private static readonly int[] Densities    = [70, 80, 90, 100, 115, 120];
    private static readonly int[] Quantities   = [50, 100, 200, 250, 500];
    private static readonly decimal[] LoadCaps = [3m, 5m, 7m, 10m, 12m, 15m];

    public static async Task SeedTestVariantsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Only run when there are fewer than 100 variants in the DB (allows re-seeding after wipe)
        if (await db.ProductVariants.CountAsync() >= 100)
            return;

        var products = await db.Products.AsNoTracking().ToListAsync();
        var materials = await db.PackageMaterials.AsNoTracking().ToListAsync();

        if (products.Count == 0 || materials.Count == 0)
            throw new InvalidOperationException(
                "TestVariantsSeeder requires Products and PackageMaterials to be seeded first.");

        var rng = new Random(42); // fixed seed → reproducible
        const int total = 1000;
        var usedSeoKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var variants = new List<ProductVariant>(total);
        var photos = new List<ProductPhoto>(total * 4);

        for (var i = 0; i < total; i++)
        {
            var product = products[i % products.Count];
            var material = materials[rng.Next(materials.Count)];
            var size = Sizes[rng.Next(Sizes.Length)];
            var density = Densities[rng.Next(Densities.Length)];
            var qty = Quantities[rng.Next(Quantities.Length)];
            var load = LoadCaps[rng.Next(LoadCaps.Length)];
            var price = Math.Round((decimal)(rng.NextDouble() * 18 + 2), 2); // 2.00–20.00 ₴

            // Build a unique slug like  "kraft-bags-200x120-80gsm-42"
            var depthPart = size.D.HasValue ? $"x{(int)size.D}" : "";
            var baseSlugUk = Slugify($"пакет-{(int)size.H}x{(int)size.W}{depthPart}-{density}gsm");
            var baseSlugEn = Slugify($"bag-{(int)size.H}x{(int)size.W}{depthPart}-{density}gsm");

            var slugUk = MakeUnique(baseSlugUk, usedSeoKeys, rng);
            var slugEn = MakeUnique(baseSlugEn, usedSeoKeys, rng);

            var variant = ProductVariant.New(
                productId: product.Id,
                packageMaterialId: material.Id,
                density: density,
                loadCapacity: load,
                seoUrl: new LocalizedString(slugUk, slugEn),
                availability: new LocalizedString("В наявності", "In Stock"),
                height: size.H,
                width: size.W,
                depth: size.D,
                pricePerPiece: price,
                quantityPerPackage: qty);

            variants.Add(variant);

            // Assign all 4 images; pick one at random to be primary
            var primaryIndex = rng.Next(TestImages.Length);
            for (var p = 0; p < TestImages.Length; p++)
            {
                photos.Add(ProductPhoto.New(variant.Id, TestImages[p], isPrimary: p == primaryIndex));
            }
        }

        await db.ProductVariants.AddRangeAsync(variants);
        await db.ProductPhotos.AddRangeAsync(photos);
        await db.SaveChangesAsync();
    }

    // ─── helpers ────────────────────────────────────────────────────────────

    private static string MakeUnique(string baseSlug, HashSet<string> used, Random rng)
    {
        var candidate = baseSlug;
        while (!used.Add(candidate))
            candidate = $"{baseSlug}-{rng.Next(1000, 9999)}";
        return candidate;
    }

    private static string Slugify(string input) =>
        System.Text.RegularExpressions.Regex
            .Replace(input.ToLowerInvariant().Trim(), @"[^a-zа-яіїєґ0-9]+", "-")
            .Trim('-');
}
