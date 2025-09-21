using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarDocument> CarDocuments { get; set; }
        public DbSet<CarApprovalStatus> CarApprovalStatuses { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingStatus> BookingStatuses { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<KYCUpload> KYCUploads { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<OTP> OTPs { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Car>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.OwnedCars)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Car>()
                .HasOne(c => c.CarApprovalStatus)
                .WithMany(s => s.Cars)
                .HasForeignKey(c => c.CarApprovalStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Car>()
                .Property(c => c.RatePerDay)
                .HasPrecision(18, 2);


            modelBuilder.Entity<CarDocument>()
                .HasOne(cd => cd.Car)
                .WithMany(c => c.Documents)
                .HasForeignKey(cd => cd.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Car)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.BookingStatus)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.BookingStatusId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.PaymentStatus)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.PaymentStatusId);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Method)
                .HasConversion<string>() // store enum as string
                .HasMaxLength(20);


            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Payment)
                .WithOne(p => p.Invoice)
                .HasForeignKey<Invoice>(i => i.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KYCUpload>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.KYCUploads)
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KYCUpload>()
                .HasOne(k => k.Customer)
                .WithMany(c => c.KYCUploads)
                .HasForeignKey(k => k.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Notification>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OTP>()
                .HasIndex(o => new { o.Email, o.Purpose })
                .HasDatabaseName("IX_OTP_Email_Purpose");

            modelBuilder.Entity<OTP>()
                .Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<OTP>()
                .Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<OTP>()
                .Property(o => o.Purpose)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
