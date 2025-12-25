using Microsoft.Extensions.DependencyInjection;

namespace MCRSolicitors.Theme;

/// <summary>
/// Minimal startup for MCR Solicitors Theme
/// All content is managed through Orchard Core's built-in fields and content types
/// </summary>
public class Startup : OrchardCore.Modules.StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // No custom services needed - using Orchard Core's built-in functionality
    }
}
