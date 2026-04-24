using JetAdminSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace JetAdminSystem.Data
{
    public class JetAdminDbContext : DbContext
    {
        public JetAdminDbContext(DbContextOptions<JetAdminDbContext> options) : base(options) { }

        public DbSet<AircraftCategory> AircraftCategories { get; set; }
        public DbSet<Aircraft> Aircrafts { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<FlightSchedule> FlightSchedules { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. Cấu hình Precision cho các cột tiền tệ (Tránh lỗi làm tròn) ---
            modelBuilder.Entity<Aircraft>().Property(a => a.PricePerHour).HasPrecision(18, 2);
            modelBuilder.Entity<Broker>().Property(b => b.CommissionRate).HasPrecision(5, 2);
            modelBuilder.Entity<Booking>().Property(b => b.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Billing>(entity => {
                entity.Property(b => b.SubTotal).HasPrecision(18, 2);
                entity.Property(b => b.TaxAmount).HasPrecision(18, 2);
                entity.Property(b => b.GrandTotal).HasPrecision(18, 2);
            });

            // --- 2. Cấu hình mối quan hệ & Fix lỗi Cascade Delete (Lỗi 1785) ---

            // Fix lỗi Multiple Cascade Paths giữa FlightSchedule và Airport
            modelBuilder.Entity<FlightSchedule>(entity =>
            {
                // Cấu hình cho DepartureAirport (Sân bay đi)
                entity.HasOne(f => f.DepartureAirport)
                    .WithMany()
                    .HasForeignKey(f => f.DepartureAirportId)
                    .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa sân bay nếu có chuyến bay đi

                // Cấu hình cho ArrivalAirport (Sân bay đến)
                entity.HasOne(f => f.ArrivalAirport)
                    .WithMany()
                    .HasForeignKey(f => f.ArrivalAirportId)
                    .OnDelete(DeleteBehavior.NoAction); // Tránh xung đột đường dẫn xóa với DepartureAirport
            });

            // Xóa Aircraft -> Xóa FlightSchedules liên quan (Giữ nguyên Cascade)
            modelBuilder.Entity<FlightSchedule>()
                .HasOne(f => f.Aircraft)
                .WithMany(a => a.FlightSchedules)
                .HasForeignKey(f => f.AircraftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking -> Schedule (Cascade)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking -> Passenger (Cascade)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Passenger)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PassengerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Aircraft -> Category (Restrict - An toàn cho danh mục)
            modelBuilder.Entity<Aircraft>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Aircrafts)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}