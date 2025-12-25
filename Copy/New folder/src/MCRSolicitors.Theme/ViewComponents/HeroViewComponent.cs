using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering hero sections
/// Content is loaded from CMS database via IContentQueryService
/// </summary>
public class HeroViewComponent : ViewComponent
{
    private readonly IContentQueryService _contentQueryService;

    public HeroViewComponent(IContentQueryService contentQueryService)
    {
        _contentQueryService = contentQueryService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? contentItemId = null)
    {
        var model = await _contentQueryService.GetHeroAsync(contentItemId);

        // Return empty view if no hero content exists in CMS
        if (model == null)
        {
            return Content(string.Empty);
        }

        return View(model);
    }
}
