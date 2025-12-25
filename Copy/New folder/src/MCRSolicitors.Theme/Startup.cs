using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using MCRSolicitors.Theme.ContentParts;
using MCRSolicitors.Theme.Drivers;
using MCRSolicitors.Theme.Services;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme;

/// <summary>
/// Startup class for registering theme services, content parts, and drivers
/// </summary>
public class Startup : OrchardCore.Modules.StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // Register services with dependency injection
        services.AddScoped<ISiteSettingsService, SiteSettingsService>();
        services.AddScoped<INavigationService, NavigationService>();
        services.AddScoped<IFooterService, FooterService>();
        services.AddScoped<IContentQueryService, ContentQueryService>();

        // Register Content Parts - Homepage sections
        services.AddContentPart<HeroPart>();
        services.AddContentPart<ServicesPart>();
        services.AddContentPart<ServiceItemPart>();
        services.AddContentPart<StatsPart>();
        services.AddContentPart<StatItemPart>();
        services.AddContentPart<FeaturesPart>();
        services.AddContentPart<FeatureItemPart>();
        services.AddContentPart<TestimonialsPart>();
        services.AddContentPart<TestimonialItemPart>();
        services.AddContentPart<CtaPart>();
        services.AddContentPart<SiteSettingsPart>();

        // Register Content Parts - Pages
        services.AddContentPart<ServiceDetailPart>();
        services.AddContentPart<ServiceContentSectionPart>();
        services.AddContentPart<FaqItemPart>();
        services.AddContentPart<TeamMemberPart>();
        services.AddContentPart<CompanyValuePart>();
        services.AddContentPart<AccreditationPart>();
        services.AddContentPart<OfficePart>();

        // Register Display Drivers - these handle admin editing and frontend display
        services.AddScoped<IContentPartDisplayDriver, HeroPartDisplayDriver>();
        services.AddScoped<IContentPartDisplayDriver, ServiceItemPartDisplayDriver>();
        services.AddScoped<IContentPartDisplayDriver, StatItemPartDisplayDriver>();
        services.AddScoped<IContentPartDisplayDriver, FeatureItemPartDisplayDriver>();
        services.AddScoped<IContentPartDisplayDriver, TestimonialItemPartDisplayDriver>();
        services.AddScoped<IContentPartDisplayDriver, CtaPartDisplayDriver>();

        // Register Migrations - these create Content Types in the database
        services.AddScoped<IDataMigration, Migrations>();
    }
}
