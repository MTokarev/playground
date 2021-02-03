using Microsoft.EntityFrameworkCore;
using playground.Entities;

namespace playground.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<EUser> Users { get; set; }
        public DbSet<ERole> Roles { get; set; }
        public DbSet<EUserActionKey> UserActionKeys{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Confiduring Cascade removing.
            // When user will be removed all associated roles will be deleted as well.
            modelBuilder.Entity<ERole>()
                .HasOne(u => u.EUser)
                .WithMany(r => r.Roles)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            // Each user might have action key associated. This could help execute task without passwords.
            // For example password reset. This required to clean all assigned keys in case user removed.
            modelBuilder.Entity<EUserActionKey>()
                .HasOne(u => u.EUser)
                .WithMany(r => r.UserActionKeys)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
