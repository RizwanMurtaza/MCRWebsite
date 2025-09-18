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
        public DbSet<EmailQueueItem> EmailQueueItems { get; set; }
        public DbSet<EmailProcessLog> EmailProcessLogs { get; set; }

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

            modelBuilder.Entity<EmailQueueItem>(entity =>
            {
                entity.HasIndex(e => e.AppId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.Status, e.Priority, e.CreatedAt });
            });

            modelBuilder.Entity<EmailProcessLog>(entity =>
            {
                entity.HasIndex(e => e.EmailQueueItemId);
                entity.HasIndex(e => e.AppId);
                entity.HasIndex(e => e.LogLevel);
                entity.HasIndex(e => e.Event);
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}