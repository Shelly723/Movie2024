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

        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasKey(e => e.OrderID);

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
        });

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
