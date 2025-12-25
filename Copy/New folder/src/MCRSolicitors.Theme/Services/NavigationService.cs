using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.Services;

/// <summary>
/// Default implementation of navigation service
/// Can be extended to pull from Orchard CMS menu system
/// </summary>
public class NavigationService : INavigationService
{
    private readonly ISiteSettingsService _siteSettings;

    public NavigationService(ISiteSettingsService siteSettings)
    {
        _siteSettings = siteSettings;
    }

    public Task<NavigationViewModel> GetNavigationAsync()
    {
        var menuItems = GetDefaultMenuItems();

        return Task.FromResult(new NavigationViewModel
        {
            LogoUrl = _siteSettings.GetLogoUrl(),
            LogoAlt = _siteSettings.GetSiteName(),
            PhoneNumber = _siteSettings.GetPhoneNumber(),
            PhoneDisplay = _siteSettings.GetPhoneNumber(),
            MenuItems = menuItems,
            ShowCtaButton = true,
            CtaText = "Free Consultation",
            CtaUrl = "/contact"
        });
    }

    public Task<List<NavigationItemModel>> GetMenuItemsAsync()
    {
        return Task.FromResult(GetDefaultMenuItems());
    }

    private static List<NavigationItemModel> GetDefaultMenuItems()
    {
        return new List<NavigationItemModel>
        {
            new() { Title = "Home", Url = "/" },
            new()
            {
                Title = "Services",
                Url = "/services",
                Children = new List<NavigationItemModel>
                {
                    new() { Title = "Family Law", Url = "/services/family-law" },
                    new() { Title = "Immigration", Url = "/services/immigration" },
                    new() { Title = "Conveyancing", Url = "/services/conveyancing" },
                    new() { Title = "Wills & Probate", Url = "/services/wills-probate" },
                    new() { Title = "Personal Injury", Url = "/services/personal-injury" },
                    new() { Title = "Employment Law", Url = "/services/employment-law" }
                }
            },
            new() { Title = "About Us", Url = "/about" },
            new() { Title = "Our Team", Url = "/team" },
            new() { Title = "Blog", Url = "/blog" },
            new() { Title = "Contact", Url = "/contact" }
        };
    }
}
