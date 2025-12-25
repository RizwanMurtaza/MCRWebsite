using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Builders;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.Flows.Models;

namespace MCRSolicitors.Theme;

public class Migrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    private static readonly string[] WidgetTypes = new[]
    {
        "HeroWidget",
        "PageHeroWidget",
        "StatsWidget",
        "ServicesWidget",
        "FeaturesWidget",
        "CtaWidget",
        "TestimonialsWidget",
        "TeamWidget",
        "FaqWidget",
        "ContentWidget"
    };

    public Migrations(IContentDefinitionManager contentDefinitionManager)
    {
        _contentDefinitionManager = contentDefinitionManager;
    }

    public async Task<int> CreateAsync()
    {
        // Update HomePage FlowPart settings
        await _contentDefinitionManager.AlterTypeDefinitionAsync("HomePage", type => type
            .WithPart("FlowPart", part => part
                .WithSettings(new FlowPartSettings
                {
                    ContainedContentTypes = WidgetTypes
                })
            )
        );

        // Update ContentPage FlowPart settings
        await _contentDefinitionManager.AlterTypeDefinitionAsync("ContentPage", type => type
            .WithPart("FlowPart", part => part
                .WithSettings(new FlowPartSettings
                {
                    ContainedContentTypes = WidgetTypes
                })
            )
        );

        return 1;
    }
}
