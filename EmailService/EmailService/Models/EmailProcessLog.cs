using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailService.Models
{
    public class EmailProcessLog
    {
        [Key]
        public int Id { get; set; }

        public int? EmailQueueItemId { get; set; }

        [MaxLength(100)]
        public string? AppId { get; set; }

        [Required]
        [MaxLength(50)]
        public string LogLevel { get; set; } // Info, Warning, Error, Critical

        [Required]
        [MaxLength(200)]
        public string Event { get; set; } // EmailQueued, EmailProcessing, EmailSent, EmailFailed, RetryAttempt, ServiceStarted, ServiceStopped, etc.

        [Required]
        [Column(TypeName = "TEXT")]
        public string Message { get; set; }

        [Column(TypeName = "TEXT")]
        public string? Details { get; set; } // JSON for additional details

        [MaxLength(500)]
        public string? ExceptionType { get; set; }

        [Column(TypeName = "TEXT")]
        public string? StackTrace { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string MachineName { get; set; } = Environment.MachineName;

        [MaxLength(100)]
        public string? ProcessName { get; set; }

        public int? ThreadId { get; set; } = System.Threading.Thread.CurrentThread.ManagedThreadId;
    }
}