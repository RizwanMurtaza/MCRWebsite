using Microsoft.EntityFrameworkCore;
using EmailService.Models;

namespace EmailService.Data
{
    public class EmailDbContext : DbContext
    {
        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
        {
        }

        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<AppCredentials> AppCredentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailLog>(entity =>
            {
                entity.HasIndex(e => e.AppId);
                entity.HasIndex(e => e.EmailType);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });

            modelBuilder.Entity<AppCredentials>(entity =>
            {
                entity.HasIndex(e => e.AppId).IsUnique();
                entity.HasIndex(e => e.AppName);
                entity.HasIndex(e => e.IsActive);
            });
        }
    }
}