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

                entity.HasData(
                    new MstrRole { Id = 1, RoleName = "Buyer" },
                    new MstrRole { Id = 2, RoleName = "Seller" }
                );
            });


            modelBuilder.Entity<MstrUser>(entity =>
            {
                entity.ToTable("MstrUser");
                entity.HasKey(e => e.Id);

                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.Password).IsRequired();

                entity.HasOne(u => u.Role)
                      .WithMany()
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
