using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using Starter.Theme.Models;

namespace Starter.Theme;

/// <summary>
/// Migration to register the ThemeSettingsPart with Orchard Core
/// </summary>
public class Migrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public Migrations(IContentDefinitionManager contentDefinitionManager)
    {
        _contentDefinitionManager = contentDefinitionManager;
    }

    public async Task<int> CreateAsync()
    {
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(ThemeSettingsPart), part => part
            .Attachable()
            .WithDescription("Provides configurable theme settings for branding, colors, and contact information.")
        );

        return 1;
    }
}
