using EmailService.DTOs;

namespace EmailService.Services
{
    public interface IEmailService
    {
        Task<bool> SendContactUsEmailAsync(ContactUsRequest request);
        Task<bool> SendEnquiryEmailAsync(EnquiryRequest request);
        Task<bool> SendEmailAsync(string appId, string toEmail, string subject, string body, string emailType, string? metadata = null);
    }
}