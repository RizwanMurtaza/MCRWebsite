using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering the main navigation
/// Single Responsibility: Only handles navigation rendering
/// </summary>
public class NavigationViewComponent : ViewComponent
{
    private readonly INavigationService _navigationService;

    public NavigationViewComponent(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _navigationService.GetNavigationAsync();

        // Mark current page as active
        var currentPath = HttpContext.Request.Path.Value?.ToLowerInvariant() ?? "/";
        MarkActiveItem(model.MenuItems, currentPath);

        return View(model);
    }

    private static void MarkActiveItem(IEnumerable<Models.NavigationItemModel> items, string currentPath)
    {
        foreach (var item in items)
        {
            item.IsActive = item.Url.Equals(currentPath, StringComparison.OrdinalIgnoreCase) ||
                           (currentPath.StartsWith(item.Url, StringComparison.OrdinalIgnoreCase) && item.Url != "/");

            if (item.HasChildren)
            {
                MarkActiveItem(item.Children, currentPath);
                // Parent is active if any child is active
                if (item.Children.Any(c => c.IsActive))
                {
                    item.IsActive = true;
                }
            }
        }
    }
}
