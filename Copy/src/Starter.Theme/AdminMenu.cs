using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Starter.Theme;

/// <summary>
/// Adds Theme Settings to admin menu
/// </summary>
public class AdminMenu : INavigationProvider
{
    private readonly IStringLocalizer S;

    public AdminMenu(IStringLocalizer<AdminMenu> localizer)
    {
        S = localizer;
    }

    public ValueTask BuildNavigationAsync(string name, NavigationBuilder builder)
    {
        if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return ValueTask.CompletedTask;
        }

        builder
            .Add(S["Configuration"], configuration => configuration
                .Add(S["Settings"], settings => settings
                    .Add(S["Theme Settings"], S["Theme Settings"].PrefixPosition(), themeSettings => themeSettings
                        .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = Drivers.ThemeSettingsSiteSettingsDisplayDriver.GroupId })
                        .Permission(Permissions.ManageThemeSettings)
                        .LocalNav()
                    )
                )
            );

        return ValueTask.CompletedTask;
    }
}
