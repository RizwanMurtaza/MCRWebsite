using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.Services;

/// <summary>
/// Default implementation of footer service
/// </summary>
public class FooterService : IFooterService
{
    private readonly ISiteSettingsService _siteSettings;

    public FooterService(ISiteSettingsService siteSettings)
    {
        _siteSettings = siteSettings;
    }

    public Task<FooterViewModel> GetFooterAsync()
    {
        return Task.FromResult(new FooterViewModel
        {
            LogoUrl = _siteSettings.GetLogoUrl(),
            LogoAlt = _siteSettings.GetSiteName(),
            CompanyDescription = "MCR Solicitors is a leading law firm in Manchester providing expert legal services with a personal touch. We are committed to delivering exceptional results for our clients.",
            Contact = new FooterContactInfo
            {
                Address = _siteSettings.GetAddress(),
                Phone = _siteSettings.GetPhoneNumber(),
                Email = _siteSettings.GetEmail(),
                OpeningHours = "Mon - Fri: 9:00 AM - 5:30 PM"
            },
            LinkSections = GetDefaultLinkSections(),
            SocialLinks = GetDefaultSocialLinks(),
            Accreditations = GetDefaultAccreditations(),
            SraNumber = _siteSettings.GetSraNumber()
        });
    }

    public Task<List<FooterLinkSection>> GetLinkSectionsAsync()
    {
        return Task.FromResult(GetDefaultLinkSections());
    }

    public Task<List<AccreditationBadge>> GetAccreditationsAsync()
    {
        return Task.FromResult(GetDefaultAccreditations());
    }

    private static List<FooterLinkSection> GetDefaultLinkSections()
    {
        return new List<FooterLinkSection>
        {
            new()
            {
                Title = "Our Services",
                Links = new List<FooterLink>
                {
                    new() { Text = "Family Law", Url = "/services/family-law" },
                    new() { Text = "Immigration", Url = "/services/immigration" },
                    new() { Text = "Conveyancing", Url = "/services/conveyancing" },
                    new() { Text = "Wills & Probate", Url = "/services/wills-probate" },
                    new() { Text = "Personal Injury", Url = "/services/personal-injury" },
                    new() { Text = "Employment Law", Url = "/services/employment-law" }
                }
            },
            new()
            {
                Title = "Quick Links",
                Links = new List<FooterLink>
                {
                    new() { Text = "About Us", Url = "/about" },
                    new() { Text = "Our Team", Url = "/team" },
                    new() { Text = "Blog", Url = "/blog" },
                    new() { Text = "Contact Us", Url = "/contact" },
                    new() { Text = "Privacy Policy", Url = "/privacy-policy" },
                    new() { Text = "Terms of Service", Url = "/terms" }
                }
            },
            new()
            {
                Title = "Resources",
                Links = new List<FooterLink>
                {
                    new() { Text = "FAQs", Url = "/faqs" },
                    new() { Text = "Legal Guides", Url = "/resources" },
                    new() { Text = "Testimonials", Url = "/testimonials" },
                    new() { Text = "Careers", Url = "/careers" }
                }
            }
        };
    }

    private static List<SocialMediaLink> GetDefaultSocialLinks()
    {
        return new List<SocialMediaLink>
        {
            new() { Platform = "Facebook", Url = "https://facebook.com/mcrsolicitors", IconClass = "fab fa-facebook-f" },
            new() { Platform = "Twitter", Url = "https://twitter.com/mcrsolicitors", IconClass = "fab fa-twitter" },
            new() { Platform = "LinkedIn", Url = "https://linkedin.com/company/mcrsolicitors", IconClass = "fab fa-linkedin-in" },
            new() { Platform = "Instagram", Url = "https://instagram.com/mcrsolicitors", IconClass = "fab fa-instagram" }
        };
    }

    private static List<AccreditationBadge> GetDefaultAccreditations()
    {
        return new List<AccreditationBadge>
        {
            new()
            {
                ImageUrl = "/MCRSolicitors.Theme/images/footerpic1.png",
                AltText = "Lexcel Accredited",
                LinkUrl = "/lexcel-accredited"
            },
            new()
            {
                ImageUrl = "/MCRSolicitors.Theme/images/sra-badge.png",
                AltText = "SRA Regulated",
                LinkUrl = "https://www.sra.org.uk"
            }
        };
    }
}
