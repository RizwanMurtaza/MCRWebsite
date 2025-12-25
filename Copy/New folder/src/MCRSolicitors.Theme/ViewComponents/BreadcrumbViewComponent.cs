using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering breadcrumb navigation
/// Important for SEO and user navigation
/// </summary>
public class BreadcrumbViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(BreadcrumbViewModel? model = null)
    {
        // If no model provided, try to get from ViewData
        if (model == null)
        {
            var breadcrumbs = ViewData["Breadcrumbs"] as List<(string Name, string Url)>;
            if (breadcrumbs != null && breadcrumbs.Any())
            {
                model = new BreadcrumbViewModel
                {
                    Items = breadcrumbs.Select((b, index) => new BreadcrumbItem
                    {
                        Name = b.Name,
                        Url = b.Url,
                        IsActive = index == breadcrumbs.Count - 1
                    }).ToList()
                };
            }
            else
            {
                // Default breadcrumb
                model = new BreadcrumbViewModel
                {
                    Items = new List<BreadcrumbItem>
                    {
                        new() { Name = "Home", Url = "/", IsActive = false }
                    }
                };
            }
        }

        return View(model);
    }
}
