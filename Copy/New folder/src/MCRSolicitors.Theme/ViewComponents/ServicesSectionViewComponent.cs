using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering services section
/// Content is loaded from CMS database via IContentQueryService
/// </summary>
public class ServicesSectionViewComponent : ViewComponent
{
    private readonly IContentQueryService _contentQueryService;

    public ServicesSectionViewComponent(IContentQueryService contentQueryService)
    {
        _contentQueryService = contentQueryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _contentQueryService.GetServicesSectionAsync();

        // Return empty if no services exist in CMS
        if (model.Services.Count == 0)
        {
            return Content(string.Empty);
        }

        return View(model);
    }
}
