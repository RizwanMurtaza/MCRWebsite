using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailService.Models
{
    public class EmailLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string AppId { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ToEmail { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? FromEmail { get; set; }

        [Required]
        [MaxLength(500)]
        public string Subject { get; set; } = string.Empty;

        [Column(TypeName = "TEXT")]
        public string Body { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string EmailType { get; set; } = string.Empty; // "ContactUs" or "Enquiry"

        public bool IsHtml { get; set; } = true;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // "Sent", "Failed", "Pending"

        public string? ErrorMessage { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "JSON")]
        public string? Metadata { get; set; } // Additional data as JSON
    }
}