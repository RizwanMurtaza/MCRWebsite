using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Settings;

namespace MCR.Theme.ViewComponents;

public class TopBarViewComponent : ViewComponent
{
    private readonly ISiteService _siteService;

    public TopBarViewComponent(ISiteService siteService)
    {
        _siteService = siteService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Get SocialLinksSettings from Site Settings (CustomSettings are stored in Properties)
        var site = await _siteService.GetSiteSettingsAsync();
        var settingsPart = site.Properties["SocialLinksSettings"]?["SocialLinksSettings"]?.AsObject();

        var model = new TopBarViewModel
        {
            // NO HARDCODED FALLBACKS - all values from CMS Site Settings
            PhoneNumber = GetFieldValue(settingsPart, "PhoneNumber"),
            PhoneNumberDisplay = GetFieldValue(settingsPart, "PhoneNumberDisplay"),
            Email = GetFieldValue(settingsPart, "Email"),
            BusinessHours = GetFieldValue(settingsPart, "BusinessHours"),
            Address = GetFieldValue(settingsPart, "Address"),
            ShowTopBar = GetBoolFieldValue(settingsPart, "ShowTopBar"),
            ShowPhone = GetBoolFieldValue(settingsPart, "ShowPhone"),
            ShowEmail = GetBoolFieldValue(settingsPart, "ShowEmail"),
            ShowHours = GetBoolFieldValue(settingsPart, "ShowHours"),
            ShowAddress = GetBoolFieldValue(settingsPart, "ShowAddress"),
            FacebookUrl = GetFieldValue(settingsPart, "FacebookUrl"),
            TwitterUrl = GetFieldValue(settingsPart, "TwitterUrl"),
            LinkedInUrl = GetFieldValue(settingsPart, "LinkedInUrl"),
            InstagramUrl = GetFieldValue(settingsPart, "InstagramUrl"),
            TikTokUrl = GetFieldValue(settingsPart, "TikTokUrl"),
            YouTubeUrl = GetFieldValue(settingsPart, "YouTubeUrl"),
            ShowSocialIcons = GetBoolFieldValue(settingsPart, "ShowSocialIcons")
        };

        return View(model);
    }

    private string? GetFieldValue(JsonObject? part, string fieldName)
    {
        if (part == null) return null;
        return part[fieldName]?["Text"]?.GetValue<string>();
    }

    private bool GetBoolFieldValue(JsonObject? part, string fieldName)
    {
        if (part == null) return false;
        return part[fieldName]?["Value"]?.GetValue<bool>() ?? false;
    }
}

public class TopBarViewModel
{
    public string? PhoneNumber { get; set; }
    public string? PhoneNumberDisplay { get; set; }
    public string? Email { get; set; }
    public string? BusinessHours { get; set; }
    public string? Address { get; set; }
    public bool ShowTopBar { get; set; }
    public bool ShowPhone { get; set; }
    public bool ShowEmail { get; set; }
    public bool ShowHours { get; set; }
    public bool ShowAddress { get; set; }
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TikTokUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public bool ShowSocialIcons { get; set; }

    public bool HasAnySocialUrl =>
        !string.IsNullOrEmpty(FacebookUrl) ||
        !string.IsNullOrEmpty(TwitterUrl) ||
        !string.IsNullOrEmpty(LinkedInUrl) ||
        !string.IsNullOrEmpty(InstagramUrl) ||
        !string.IsNullOrEmpty(TikTokUrl) ||
        !string.IsNullOrEmpty(YouTubeUrl);
}
