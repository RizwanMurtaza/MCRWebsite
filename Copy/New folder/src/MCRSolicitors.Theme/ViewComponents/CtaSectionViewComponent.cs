using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering call-to-action sections
/// Content is loaded from CMS database via IContentQueryService
/// </summary>
public class CtaSectionViewComponent : ViewComponent
{
    private readonly IContentQueryService _contentQueryService;

    public CtaSectionViewComponent(IContentQueryService contentQueryService)
    {
        _contentQueryService = contentQueryService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? contentItemId = null)
    {
        var model = await _contentQueryService.GetCtaSectionAsync(contentItemId);

        // Return empty if no CTA content exists in CMS
        if (model == null)
        {
            return Content(string.Empty);
        }

        return View(model);
    }
}
