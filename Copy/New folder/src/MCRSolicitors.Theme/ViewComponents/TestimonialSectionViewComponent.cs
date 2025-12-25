using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering testimonials section
/// Content is loaded from CMS database via IContentQueryService
/// </summary>
public class TestimonialSectionViewComponent : ViewComponent
{
    private readonly IContentQueryService _contentQueryService;

    public TestimonialSectionViewComponent(IContentQueryService contentQueryService)
    {
        _contentQueryService = contentQueryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _contentQueryService.GetTestimonialsSectionAsync();

        // Return empty if no testimonials exist in CMS
        if (model.Testimonials.Count == 0)
        {
            return Content(string.Empty);
        }

        return View(model);
    }
}
