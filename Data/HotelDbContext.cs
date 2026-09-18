using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingRoom> BookingRooms => Set<BookingRoom>();

    public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>()
            .HasIndex(r => r.RoomNumber)
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.BookingNo)
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Customer)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BookingRoom>()
            .HasIndex(br => new { br.BookingId, br.RoomId })
            .IsUnique();

        modelBuilder.Entity<BookingRoom>()
            .HasOne(br => br.Booking)
            .WithMany(b => b.BookingRooms)
            .HasForeignKey(br => br.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookingRoom>()
            .HasOne(br => br.Room)
            .WithMany(r => r.BookingRooms)
            .HasForeignKey(br => br.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MenuCategory>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<MenuItem>()
            .HasOne(m => m.MenuCategory)
            .WithMany(c => c.MenuItems)
            .HasForeignKey(m => m.MenuCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RestaurantTable>()
            .HasIndex(t => t.TableNumber)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .HasOne(o => o.RestaurantTable)
            .WithMany(t => t.Orders)
            .HasForeignKey(o => o.RestaurantTableId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Booking)
            .WithMany(b => b.Orders)
            .HasForeignKey(o => o.BookingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .HasForeignKey(oi => oi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserAccount>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Room>().Property(x => x.PricePerNight).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(x => x.RoomCharge).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(x => x.RestaurantBill).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(x => x.BarBill).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(x => x.LaundryBill).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(x => x.SeasonalChargePercent).HasPrecision(8, 2);
        modelBuilder.Entity<Booking>().Property(x => x.SeasonalChargeAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(x => x.Discount).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(x => x.AdvancePayment).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>().Property(x => x.GrandTotal).HasPrecision(18, 2);
        modelBuilder.Entity<MenuItem>().Property(x => x.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(x => x.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(x => x.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(x => x.TotalPrice).HasPrecision(18, 2);
    }
}
