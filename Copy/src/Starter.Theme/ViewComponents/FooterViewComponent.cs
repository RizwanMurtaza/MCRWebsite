using Microsoft.AspNetCore.Mvc;
using Starter.Theme.Services.Abstractions;

namespace Starter.Theme.ViewComponents;

/// <summary>
/// ViewComponent for rendering the footer
/// Uses theme settings for contact info and social links
/// </summary>
public class FooterViewComponent : ViewComponent
{
    private readonly IThemeSettingsService _settingsService;

    public FooterViewComponent(IThemeSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        return View(settings);
    }
}
