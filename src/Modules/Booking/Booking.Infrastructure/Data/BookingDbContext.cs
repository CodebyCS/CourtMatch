using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Booking.Infrastructure.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.Entities.Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Domain.Entities.Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                
                entity.Property(b => b.CourtId).IsRequired();

                entity.Property(b => b.HostPlayerId).IsRequired();

                entity.Property(b => b.Schedule).IsRequired();

                entity.Property(b => b.TotalPrice)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(b => b.Status)
                    .HasConversion<string>()
                    .IsRequired();
            });
        }
    }
}
