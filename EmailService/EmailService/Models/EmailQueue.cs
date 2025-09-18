using System;
using System.ComponentModel.DataAnnotations;

namespace EmailService.Models
{
    public class EmailQueue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string AppId { get; set; }

        [Required]
        [EmailAddress]
        public string ToEmail { get; set; }

        [MaxLength(200)]
        public string ToName { get; set; }

        [EmailAddress]
        public string CcEmail { get; set; }

        [EmailAddress]
        public string BccEmail { get; set; }

        [Required]
        [MaxLength(500)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public bool IsHtml { get; set; } = true;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Sent, Failed

        public int RetryCount { get; set; } = 0;

        public int MaxRetries { get; set; } = 3;

        [MaxLength(500)]
        public string LastError { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? FailedAt { get; set; }

        [MaxLength(50)]
        public string Priority { get; set; } = "Normal"; // Low, Normal, High

        [MaxLength(2000)]
        public string AttachmentPaths { get; set; } // JSON array of file paths

        [MaxLength(100)]
        public string ClientIp { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; }

        [MaxLength(100)]
        public string RequestId { get; set; }
    }
}