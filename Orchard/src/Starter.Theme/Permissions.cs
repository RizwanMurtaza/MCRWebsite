using OrchardCore.Security.Permissions;

namespace Starter.Theme;

/// <summary>
/// Permissions for theme settings management
/// </summary>
public class Permissions : IPermissionProvider
{
    public static readonly Permission ManageThemeSettings = new("ManageThemeSettings", "Manage Theme Settings");

    public Task<IEnumerable<Permission>> GetPermissionsAsync()
    {
        return Task.FromResult<IEnumerable<Permission>>(new[]
        {
            ManageThemeSettings
        });
    }

    public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
    {
        return new[]
        {
            new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = new[] { ManageThemeSettings }
            }
        };
    }
}
