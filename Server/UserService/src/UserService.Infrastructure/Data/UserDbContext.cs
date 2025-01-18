using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<MstrRole> MstrRoles { get; set; } = default!;
        public DbSet<MstrUser> MstrUsers { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MstrRole>(entity =>
            {
                entity.ToTable("MstrRole");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoleName).IsRequired();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedBy).HasDefaultValue("SYS");
                entity.Property(e => e.ModifiedOn).IsRequired(false);
                entity.Property(e => e.ModifiedBy).IsRequired(false);

                entity.HasData(
                    new MstrRole
                    {
                        Id = 1,
                        RoleName = "Buyer",
                        CreatedOn = new DateTime(2025, 1, 1),
                        CreatedBy = "SYS"
                    },
                    new MstrRole
                    {
                        Id = 2,
                        RoleName = "Seller",
                        CreatedOn = new DateTime(2025, 1, 1),
                        CreatedBy = "SYS"
                    }
                );
            });

            modelBuilder.Entity<MstrUser>(entity =>
            {
                entity.ToTable("MstrUser");
                entity.HasKey(e => e.Id);

                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.SecurityStamp).IsRequired();

                entity.Property(u => u.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(u => u.CreatedBy).HasDefaultValue("SYS");
                entity.Property(u => u.ModifiedOn).IsRequired(false);
                entity.Property(u => u.ModifiedBy).IsRequired(false);

                entity.HasOne(u => u.Role)
                      .WithMany()
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
