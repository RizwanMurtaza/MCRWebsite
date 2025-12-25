using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using Starter.Theme.Models;
using Starter.Theme.ViewModels;

namespace Starter.Theme.Drivers;

/// <summary>
/// Display driver for ThemeSettingsPart - renders the admin UI for settings
/// </summary>
public class ThemeSettingsDisplayDriver : ContentPartDisplayDriver<ThemeSettingsPart>
{
    private readonly IStringLocalizer<ThemeSettingsDisplayDriver> S;

    public ThemeSettingsDisplayDriver(IStringLocalizer<ThemeSettingsDisplayDriver> localizer)
    {
        S = localizer;
    }

    public override IDisplayResult Edit(ThemeSettingsPart part, BuildPartEditorContext context)
    {
        return Initialize<ThemeSettingsViewModel>("ThemeSettingsPart_Edit", model =>
        {
            model.LogoUrl = part.LogoUrl;
            model.FaviconUrl = part.FaviconUrl;
            model.SiteName = part.SiteName;
            model.Tagline = part.Tagline;
            model.PrimaryColor = part.PrimaryColor;
            model.PrimaryHoverColor = part.PrimaryHoverColor;
            model.SecondaryColor = part.SecondaryColor;
            model.HeadingFont = part.HeadingFont;
            model.BodyFont = part.BodyFont;
            model.Phone = part.Phone;
            model.Email = part.Email;
            model.Address = part.Address;
            model.OpeningHours = part.OpeningHours;
            model.FacebookUrl = part.FacebookUrl;
            model.TwitterUrl = part.TwitterUrl;
            model.LinkedInUrl = part.LinkedInUrl;
            model.InstagramUrl = part.InstagramUrl;
            model.FooterDescription = part.FooterDescription;
            model.FooterAbout = part.FooterAbout;
            model.FooterBadgesHtml = part.FooterBadgesHtml;
            model.CopyrightText = part.CopyrightText;
            model.AccreditationLogo1Url = part.AccreditationLogo1Url;
            model.AccreditationLogo1Alt = part.AccreditationLogo1Alt;
            model.AccreditationLogo1Link = part.AccreditationLogo1Link;
            model.AccreditationLogo2Url = part.AccreditationLogo2Url;
            model.AccreditationLogo2Alt = part.AccreditationLogo2Alt;
            model.AccreditationLogo2Link = part.AccreditationLogo2Link;
            model.HeroTitle = part.HeroTitle;
            model.HeroSubtitle = part.HeroSubtitle;
            model.HeroDescription = part.HeroDescription;
            model.HeroBackgroundUrl = part.HeroBackgroundUrl;
            model.HeroButtonText = part.HeroButtonText;
            model.HeroButtonUrl = part.HeroButtonUrl;
            model.HeroPrimaryButtonText = part.HeroPrimaryButtonText;
            model.HeroPrimaryButtonUrl = part.HeroPrimaryButtonUrl;
            model.HeroSecondaryButtonText = part.HeroSecondaryButtonText;
            model.HeroSecondaryButtonUrl = part.HeroSecondaryButtonUrl;
        }).Location("Content:1");
    }

    public override async Task<IDisplayResult> UpdateAsync(ThemeSettingsPart part, UpdatePartEditorContext context)
    {
        var model = new ThemeSettingsViewModel();

        if (await context.Updater.TryUpdateModelAsync(model, Prefix))
        {
            part.LogoUrl = model.LogoUrl;
            part.FaviconUrl = model.FaviconUrl;
            part.SiteName = model.SiteName;
            part.Tagline = model.Tagline;
            part.PrimaryColor = model.PrimaryColor;
            part.PrimaryHoverColor = model.PrimaryHoverColor;
            part.SecondaryColor = model.SecondaryColor;
            part.HeadingFont = model.HeadingFont;
            part.BodyFont = model.BodyFont;
            part.Phone = model.Phone;
            part.Email = model.Email;
            part.Address = model.Address;
            part.OpeningHours = model.OpeningHours;
            part.FacebookUrl = model.FacebookUrl;
            part.TwitterUrl = model.TwitterUrl;
            part.LinkedInUrl = model.LinkedInUrl;
            part.InstagramUrl = model.InstagramUrl;
            part.FooterDescription = model.FooterDescription;
            part.FooterAbout = model.FooterAbout;
            part.FooterBadgesHtml = model.FooterBadgesHtml;
            part.CopyrightText = model.CopyrightText;
            part.AccreditationLogo1Url = model.AccreditationLogo1Url;
            part.AccreditationLogo1Alt = model.AccreditationLogo1Alt;
            part.AccreditationLogo1Link = model.AccreditationLogo1Link;
            part.AccreditationLogo2Url = model.AccreditationLogo2Url;
            part.AccreditationLogo2Alt = model.AccreditationLogo2Alt;
            part.AccreditationLogo2Link = model.AccreditationLogo2Link;
            part.HeroTitle = model.HeroTitle;
            part.HeroSubtitle = model.HeroSubtitle;
            part.HeroDescription = model.HeroDescription;
            part.HeroBackgroundUrl = model.HeroBackgroundUrl;
            part.HeroButtonText = model.HeroButtonText;
            part.HeroButtonUrl = model.HeroButtonUrl;
            part.HeroPrimaryButtonText = model.HeroPrimaryButtonText;
            part.HeroPrimaryButtonUrl = model.HeroPrimaryButtonUrl;
            part.HeroSecondaryButtonText = model.HeroSecondaryButtonText;
            part.HeroSecondaryButtonUrl = model.HeroSecondaryButtonUrl;
        }

        return Edit(part, context);
    }
}
