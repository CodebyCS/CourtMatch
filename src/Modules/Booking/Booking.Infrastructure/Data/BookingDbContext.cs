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

                entity.Property(b => b.StartTime).IsRequired();
                entity.Property(b => b.EndTime).IsRequired();

                entity.Property(b => b.CourtPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(b => b.TotalPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(b => b.Status)
                    .HasConversion<string>() 
                    .IsRequired();

                entity.HasMany(b => b.Equipments)
                    .WithOne()
                    .HasForeignKey(be => be.BookingId)
                    .OnDelete(DeleteBehavior.Cascade); 

                entity.Metadata.FindNavigation(nameof(Domain.Entities.Booking.Equipments))
                    ?.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<BookingEquipment>(entity =>
            {
                entity.HasKey(be => be.Id);
                entity.Property(be => be.BookingId).IsRequired();
                entity.Property(be => be.EquipmentId).IsRequired();
                entity.Property(be => be.Quantity).IsRequired();

                entity.Property(be => be.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(be => be.TotalPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });
        }
    }
}