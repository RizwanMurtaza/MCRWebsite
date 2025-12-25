using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;
using Starter.Theme.Models;
using Starter.Theme.ViewModels;

namespace Starter.Theme.Drivers;

/// <summary>
/// Site Settings display driver - shows ThemeSettingsPart in Admin Settings
/// </summary>
public class ThemeSettingsSiteSettingsDisplayDriver : SectionDisplayDriver<ISite, ThemeSettingsPart>
{
    public const string GroupId = "StarterTheme";

    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ThemeSettingsSiteSettingsDisplayDriver(
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor)
    {
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<IDisplayResult?> EditAsync(ISite site, ThemeSettingsPart section, BuildEditorContext context)
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !await _authorizationService.AuthorizeAsync(user, Permissions.ManageThemeSettings))
        {
            return null;
        }

        return Initialize<ThemeSettingsViewModel>("ThemeSettingsPart_Edit", vm =>
        {
            vm.LogoUrl = section.LogoUrl;
            vm.FaviconUrl = section.FaviconUrl;
            vm.SiteName = section.SiteName;
            vm.Tagline = section.Tagline;
            vm.PrimaryColor = section.PrimaryColor;
            vm.PrimaryHoverColor = section.PrimaryHoverColor;
            vm.SecondaryColor = section.SecondaryColor;
            vm.HeadingFont = section.HeadingFont;
            vm.BodyFont = section.BodyFont;
            vm.Phone = section.Phone;
            vm.Email = section.Email;
            vm.Address = section.Address;
            vm.OpeningHours = section.OpeningHours;
            vm.FacebookUrl = section.FacebookUrl;
            vm.TwitterUrl = section.TwitterUrl;
            vm.LinkedInUrl = section.LinkedInUrl;
            vm.InstagramUrl = section.InstagramUrl;
            vm.FooterDescription = section.FooterDescription;
            vm.FooterAbout = section.FooterAbout;
            vm.FooterBadgesHtml = section.FooterBadgesHtml;
            vm.CopyrightText = section.CopyrightText;
            vm.AccreditationLogo1Url = section.AccreditationLogo1Url;
            vm.AccreditationLogo1Alt = section.AccreditationLogo1Alt;
            vm.AccreditationLogo1Link = section.AccreditationLogo1Link;
            vm.AccreditationLogo2Url = section.AccreditationLogo2Url;
            vm.AccreditationLogo2Alt = section.AccreditationLogo2Alt;
            vm.AccreditationLogo2Link = section.AccreditationLogo2Link;
            vm.HeroTitle = section.HeroTitle;
            vm.HeroSubtitle = section.HeroSubtitle;
            vm.HeroDescription = section.HeroDescription;
            vm.HeroBackgroundUrl = section.HeroBackgroundUrl;
            vm.HeroButtonText = section.HeroButtonText;
            vm.HeroButtonUrl = section.HeroButtonUrl;
            vm.HeroPrimaryButtonText = section.HeroPrimaryButtonText;
            vm.HeroPrimaryButtonUrl = section.HeroPrimaryButtonUrl;
            vm.HeroSecondaryButtonText = section.HeroSecondaryButtonText;
            vm.HeroSecondaryButtonUrl = section.HeroSecondaryButtonUrl;
        }).Location("Content:5").OnGroup(GroupId);
    }

    public override async Task<IDisplayResult?> UpdateAsync(ISite site, ThemeSettingsPart section, UpdateEditorContext context)
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !await _authorizationService.AuthorizeAsync(user, Permissions.ManageThemeSettings))
        {
            return null;
        }

        if (context.GroupId == GroupId)
        {
            var vm = new ThemeSettingsViewModel();

            await context.Updater.TryUpdateModelAsync(vm, Prefix);

            section.LogoUrl = vm.LogoUrl;
            section.FaviconUrl = vm.FaviconUrl;
            section.SiteName = vm.SiteName;
            section.Tagline = vm.Tagline;
            section.PrimaryColor = vm.PrimaryColor;
            section.PrimaryHoverColor = vm.PrimaryHoverColor;
            section.SecondaryColor = vm.SecondaryColor;
            section.HeadingFont = vm.HeadingFont;
            section.BodyFont = vm.BodyFont;
            section.Phone = vm.Phone;
            section.Email = vm.Email;
            section.Address = vm.Address;
            section.OpeningHours = vm.OpeningHours;
            section.FacebookUrl = vm.FacebookUrl;
            section.TwitterUrl = vm.TwitterUrl;
            section.LinkedInUrl = vm.LinkedInUrl;
            section.InstagramUrl = vm.InstagramUrl;
            section.FooterDescription = vm.FooterDescription;
            section.FooterAbout = vm.FooterAbout;
            section.FooterBadgesHtml = vm.FooterBadgesHtml;
            section.CopyrightText = vm.CopyrightText;
            section.AccreditationLogo1Url = vm.AccreditationLogo1Url;
            section.AccreditationLogo1Alt = vm.AccreditationLogo1Alt;
            section.AccreditationLogo1Link = vm.AccreditationLogo1Link;
            section.AccreditationLogo2Url = vm.AccreditationLogo2Url;
            section.AccreditationLogo2Alt = vm.AccreditationLogo2Alt;
            section.AccreditationLogo2Link = vm.AccreditationLogo2Link;
            section.HeroTitle = vm.HeroTitle;
            section.HeroSubtitle = vm.HeroSubtitle;
            section.HeroDescription = vm.HeroDescription;
            section.HeroBackgroundUrl = vm.HeroBackgroundUrl;
            section.HeroButtonText = vm.HeroButtonText;
            section.HeroButtonUrl = vm.HeroButtonUrl;
            section.HeroPrimaryButtonText = vm.HeroPrimaryButtonText;
            section.HeroPrimaryButtonUrl = vm.HeroPrimaryButtonUrl;
            section.HeroSecondaryButtonText = vm.HeroSecondaryButtonText;
            section.HeroSecondaryButtonUrl = vm.HeroSecondaryButtonUrl;
        }

        return await EditAsync(site, section, context);
    }
}
