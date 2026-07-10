using Microsoft.EntityFrameworkCore;

namespace FacilitiesCatalog.API
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("catalog");
        
            base.OnModelCreating(modelBuilder);
        }
    }
}
