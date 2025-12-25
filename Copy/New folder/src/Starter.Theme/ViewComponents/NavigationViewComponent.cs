using Microsoft.AspNetCore.Mvc;
using Starter.Theme.Services.Abstractions;

namespace Starter.Theme.ViewComponents;

/// <summary>
/// ViewComponent for rendering the navigation header
/// Uses theme settings for logo and branding
/// </summary>
public class NavigationViewComponent : ViewComponent
{
    private readonly IThemeSettingsService _settingsService;

    public NavigationViewComponent(IThemeSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        return View(settings);
    }
}
