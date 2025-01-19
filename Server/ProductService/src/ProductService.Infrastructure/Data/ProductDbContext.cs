using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<MstrCategory> MstrCategories { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MstrCategory>(entity =>
            {
                entity.ToTable("MstrCategory");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CategoryName).IsRequired();

                entity.Property(e => e.CreatedOn).HasDefaultValue(new DateTime(2025, 1, 1));
                entity.Property(e => e.CreatedBy).HasDefaultValue("SYS");
                entity.Property(e => e.ModifiedOn).IsRequired(false);
                entity.Property(e => e.ModifiedBy).IsRequired(false);

                entity.HasData(
                    new MstrCategory { Id = 1, CategoryName = "Technology" },
                    new MstrCategory { Id = 2, CategoryName = "Fashion" },
                    new MstrCategory { Id = 3, CategoryName = "Food & Drink" },
                    new MstrCategory { Id = 4, CategoryName = "Others" }
                );
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CreatedBy).IsRequired();

                entity.Property(e => e.ModifiedOn).IsRequired(false);
                entity.Property(e => e.ModifiedBy).IsRequired(false);

                entity.HasOne(p => p.Category)
                      .WithMany()
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
