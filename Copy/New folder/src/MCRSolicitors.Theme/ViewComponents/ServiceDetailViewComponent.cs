using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering individual service detail pages
/// </summary>
public class ServiceDetailViewComponent : ViewComponent
{
    private readonly ISiteSettingsService _siteSettings;

    public ServiceDetailViewComponent(ISiteSettingsService siteSettings)
    {
        _siteSettings = siteSettings;
    }

    public IViewComponentResult Invoke(ServiceDetailViewModel model)
    {
        return View(model);
    }
}
