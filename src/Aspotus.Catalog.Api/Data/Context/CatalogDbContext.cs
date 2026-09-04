using Aspotus.Catalog.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Data.Context
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
        {
        }

        public DbSet<CarBrand> CarBrands => Set<CarBrand>();
        public DbSet<CarModel> CarModels => Set<CarModel>();
        public DbSet<CarGeneration> CarGenerations => Set<CarGeneration>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<CarImage> CarImages => Set<CarImage>();
        public DbSet<PartReplacement> PartReplacements => Set<PartReplacement>();
        public DbSet<PartCategory> PartCategories => Set<PartCategory>();
        public DbSet<PartManufacturer> PartManufacturers => Set<PartManufacturer>();
        public DbSet<Part> Parts => Set<Part>();
        public DbSet<PartImage> PartImages => Set<PartImage>();

        public DbSet<PartCompatibility> PartCompatibilities => Set<PartCompatibility>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
