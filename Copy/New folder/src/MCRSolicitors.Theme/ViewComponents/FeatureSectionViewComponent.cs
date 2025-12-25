using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering why choose us / features section
/// Content is loaded from CMS database via IContentQueryService
/// </summary>
public class FeatureSectionViewComponent : ViewComponent
{
    private readonly IContentQueryService _contentQueryService;

    public FeatureSectionViewComponent(IContentQueryService contentQueryService)
    {
        _contentQueryService = contentQueryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _contentQueryService.GetFeaturesSectionAsync();

        // Return empty if no features exist in CMS
        if (model.Features.Count == 0)
        {
            return Content(string.Empty);
        }

        return View(model);
    }
}
