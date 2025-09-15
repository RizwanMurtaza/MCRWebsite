using System.ComponentModel.DataAnnotations;

namespace EmailService.Models
{
    public class AppCredentials
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string AppId { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string AppName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string SmtpHost { get; set; } = string.Empty;

        [Required]
        public int SmtpPort { get; set; }

        [Required]
        [MaxLength(255)]
        public string SmtpUsername { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string SmtpPassword { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FromEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FromName { get; set; } = string.Empty;

        public bool EnableSsl { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}