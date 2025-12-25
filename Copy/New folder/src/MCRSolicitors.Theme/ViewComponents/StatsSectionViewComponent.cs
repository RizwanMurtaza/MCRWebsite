using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering statistics section
/// Content is loaded from CMS database via IContentQueryService
/// </summary>
public class StatsSectionViewComponent : ViewComponent
{
    private readonly IContentQueryService _contentQueryService;

    public StatsSectionViewComponent(IContentQueryService contentQueryService)
    {
        _contentQueryService = contentQueryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _contentQueryService.GetStatsSectionAsync();

        // Return empty if no stats exist in CMS
        if (model.Stats.Count == 0)
        {
            return Content(string.Empty);
        }

        return View(model);
    }
}
