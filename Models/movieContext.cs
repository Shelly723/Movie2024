using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Movie2024.Models;

public partial class movieContext : DbContext
{
    public movieContext(DbContextOptions<movieContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Movies> Movies { get; set; }

    public virtual DbSet<OrderSeats> OrderSeats { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<Seats> Seats { get; set; }

    public virtual DbSet<Showtimes> Showtimes { get; set; }

    public virtual DbSet<ShowtimeSeats> ShowtimeSeats { get; set; }

    public virtual DbSet<Theaters> Theaters { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movies>(entity =>
        {
            entity.HasKey(e => e.MovieID);

            entity.Property(e => e.MovieDescription).HasMaxLength(100);
            entity.Property(e => e.MovieGenre).HasMaxLength(20);
            entity.Property(e => e.MoviePicture).HasMaxLength(100);
            entity.Property(e => e.MovieTitle).HasMaxLength(100);
        });

        modelBuilder.Entity<OrderSeats>(entity =>
        {
            entity.HasKey(e => e.OrderSeatID).HasName("PK_OrderSeats_1");
        });
        modelBuilder.Entity<OrderSeats>()
            .HasOne(os => os.Orders)
            .WithMany(o => o.OrderSeats)
            .HasForeignKey(os => os.OrderID)
            .HasConstraintName("FK_OrderSeats_Orders");  // 明確指定外鍵名稱

        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasKey(e => e.OrderID);

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
        });
     

        // 設定 Orders 和 Showtimes 之間的關聯
        modelBuilder.Entity<Orders>()
            .HasOne(o => o.Showtimes)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.ShowtimeID);

        modelBuilder.Entity<OrderSeats>()
            .HasOne(os => os.Seats)
            .WithMany(s => s.OrderSeats)
            .HasForeignKey(os => os.SeatID)
            .HasConstraintName("FK_OrderSeats_Seats");  // 明確指定外鍵名稱

        modelBuilder.Entity<Seats>(entity =>
        {
            entity.HasKey(e => e.SeatID).HasName("PK_OrderSeats");

            entity.Property(e => e.IsAvailable).HasMaxLength(5);
            entity.Property(e => e.SeatNumber).HasMaxLength(10);
        });

        modelBuilder.Entity<Showtimes>(entity =>
        {
            entity.HasKey(e => e.ShowtimeID);

            entity.Property(e => e.ShowDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Showtimes>()
       .HasOne(s => s.Movies)
       .WithMany(m => m.Showtimes)
       .HasForeignKey(s => s.MovieID);

        modelBuilder.Entity<ShowtimeSeats>(entity =>
        {
            entity.HasKey(e => e.ShowtimeSeatID);

            entity.Property(e => e.IsAvailable).HasMaxLength(2);
        });

        modelBuilder.Entity<ShowtimeSeats>()
            .HasOne(ss => ss.Showtimes)
            .WithMany(s => s.ShowtimeSeats)  // 多個 ShowtimeSeats 屬於一個 Showtime
            .HasForeignKey(ss => ss.ShowtimeID);

        modelBuilder.Entity<ShowtimeSeats>()
            .HasOne(ss => ss.Seats)
            .WithMany(s => s.ShowtimeSeats)  // 多個 ShowtimeSeats 屬於一個 Seat
            .HasForeignKey(ss => ss.SeatID);
       

        modelBuilder.Entity<Theaters>(entity =>
        {
            entity.HasKey(e => e.TheaterID);

            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.TheaterName).HasMaxLength(50);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.UserID);

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.isStaff).HasMaxLength(5);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
