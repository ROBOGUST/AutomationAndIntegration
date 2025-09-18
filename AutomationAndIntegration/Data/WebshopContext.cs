using Microsoft.EntityFrameworkCore;
using AutomationAndIntegration.Models;
using System;
using System.IO;
using System.Linq;

namespace AutomationAndIntegration.Data
{
    public class WebshopContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<IntegrationLog> IntegrationLogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var envPath = Environment.GetEnvironmentVariable("WEBSHOP_DB");
            if (!string.IsNullOrEmpty(envPath))
            {
                optionsBuilder.UseSqlite($"Data Source={envPath}");
                return;
            }

            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            DirectoryInfo? root = dir;
            while (root != null && !root.GetFiles("*.sln").Any())
            {
                root = root.Parent;
            }

            string dbPath;
            if (root != null)
            {
                string dataFolder = Path.Combine(root.FullName, "Data");
                Directory.CreateDirectory(dataFolder);
                dbPath = Path.Combine(dataFolder, "webshop.db");
            }
            else
            {
                dbPath = Path.Combine(AppContext.BaseDirectory, "webshop.db");
            }

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
            modelBuilder.Entity<IntegrationLog>().ToTable("IntegrationLogs");

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId);
        }
    }
}
