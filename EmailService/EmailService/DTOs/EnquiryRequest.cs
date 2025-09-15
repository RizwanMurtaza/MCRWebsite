using System.ComponentModel.DataAnnotations;

namespace EmailService.DTOs
{
    public class EnquiryRequest
    {
        [Required]
        public string AppId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(255)]
        public string ServiceType { get; set; } = string.Empty; // Immigration, Family Law, etc.

        [Required]
        [MaxLength(2000)]
        public string Enquiry { get; set; } = string.Empty;

        public string? PreferredContactMethod { get; set; } // Email, Phone

        public DateTime? PreferredContactTime { get; set; }
    }
}