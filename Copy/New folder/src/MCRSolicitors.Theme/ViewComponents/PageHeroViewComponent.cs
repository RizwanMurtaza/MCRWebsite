using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering inner page hero sections
/// Used on all pages except the home page
/// </summary>
public class PageHeroViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string title, string? description = null, string? backgroundImageUrl = null)
    {
        var model = new PageHeroViewModel
        {
            Title = title,
            Description = description,
            BackgroundImageUrl = backgroundImageUrl,
            ShowPattern = string.IsNullOrEmpty(backgroundImageUrl)
        };

        return View(model);
    }
}
