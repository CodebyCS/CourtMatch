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

        public DbSet<BookingEquipment> BookingEquipments { get; set; }

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

            modelBuilder.Entity<BookingEquipment>(entity =>
            {
                entity.HasKey(be => be.Id);
                entity.Property(be => be.BookingId).IsRequired();
                entity.Property(be => be.EquipmentId).IsRequired();
                entity.Property(be => be.Quantity).IsRequired();
                entity.Property(be => be.TotalPrice)
                    .HasConversion<string>()
                    .IsRequired();
            });
        }
    }
}
