using Microsoft.AspNetCore.Mvc;
using Starter.Theme.Services.Abstractions;

namespace Starter.Theme.ViewComponents;

/// <summary>
/// ViewComponent for rendering the hero section
/// Uses theme settings for hero content
/// </summary>
public class HeroViewComponent : ViewComponent
{
    private readonly IThemeSettingsService _settingsService;

    public HeroViewComponent(IThemeSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        return View(settings);
    }
}
