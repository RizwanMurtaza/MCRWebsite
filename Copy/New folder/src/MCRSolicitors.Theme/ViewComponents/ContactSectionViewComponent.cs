using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering contact section with enquiry form
/// </summary>
public class ContactSectionViewComponent : ViewComponent
{
    private readonly ISiteSettingsService _siteSettings;

    public ContactSectionViewComponent(ISiteSettingsService siteSettings)
    {
        _siteSettings = siteSettings;
    }

    public IViewComponentResult Invoke(ContactSectionViewModel? model = null)
    {
        model ??= GetDefaultContactModel();
        return View(model);
    }

    private ContactSectionViewModel GetDefaultContactModel()
    {
        return new ContactSectionViewModel
        {
            SectionTitle = "Get In Touch",
            SectionSubtitle = "Contact Us",
            SectionDescription = "Have a legal question or need expert advice? Our team is here to help. Fill out the form below or contact us directly.",
            ContactDetails = new ContactInfo
            {
                Address = "123 Deansgate",
                AddressLine2 = "Suite 500",
                City = "Manchester",
                PostCode = "M3 2BW",
                Phone = _siteSettings.GetPhoneNumber(),
                Email = _siteSettings.GetEmail(),
                OpeningHours = new Dictionary<string, string>
                {
                    { "Monday - Friday", "9:00 AM - 5:30 PM" },
                    { "Saturday", "10:00 AM - 2:00 PM" },
                    { "Sunday", "Closed" }
                }
            },
            ShowMap = true,
            MapEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d2374.2857841393447!2d-2.2448!3d53.4808!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x487bb1c8e1f8d4d5%3A0x123456789!2sDeansgate%2C%20Manchester!5e0!3m2!1sen!2suk!4v1234567890",
            ShowContactForm = true,
            Layout = ContactLayout.SplitForm
        };
    }
}
