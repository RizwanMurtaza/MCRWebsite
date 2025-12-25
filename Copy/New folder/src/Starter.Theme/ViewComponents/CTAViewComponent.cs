using Microsoft.AspNetCore.Mvc;
using Starter.Theme.Services.Abstractions;

namespace Starter.Theme.ViewComponents;

/// <summary>
/// ViewComponent for rendering Call-to-Action sections
/// Uses theme settings for CTA content
/// </summary>
public class CTAViewComponent : ViewComponent
{
    private readonly IThemeSettingsService _settingsService;

    public CTAViewComponent(IThemeSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        return View(settings);
    }
}
