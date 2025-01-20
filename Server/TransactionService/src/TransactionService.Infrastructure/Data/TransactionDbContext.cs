using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;

namespace TransactionService.Infrastructure.Data
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        {
        }

        public DbSet<TransactionHistory> TransactionHistories { get; set; } = default!;
        public DbSet<MstrStatus> MstrStatuses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MstrStatus Table
            modelBuilder.Entity<MstrStatus>(entity =>
            {
                entity.ToTable("MstrStatus");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StatusName).IsRequired();
                entity.Property(e => e.CreatedOn).HasDefaultValue(new DateTime(2025, 1, 1));
                entity.Property(e => e.CreatedBy).HasDefaultValue("SYS");
            });

            // TransactionHistory Table
            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.ToTable("TransactionHistory");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ProductId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.TotalPrice).IsRequired();
                entity.Property(e => e.TransactionAt)
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.ModifiedOn)
                      .HasColumnType("timestamp with time zone");
                entity.Property(e => e.StatusId).IsRequired();

                entity.HasOne(th => th.Status)
                      .WithMany()
                      .HasForeignKey(th => th.StatusId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Remarks).IsRequired(false);
            });
        }
    }
}
