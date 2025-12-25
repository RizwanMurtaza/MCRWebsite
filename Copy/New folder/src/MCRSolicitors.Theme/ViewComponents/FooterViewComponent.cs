using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering the footer
/// Single Responsibility: Only handles footer rendering
/// </summary>
public class FooterViewComponent : ViewComponent
{
    private readonly IFooterService _footerService;

    public FooterViewComponent(IFooterService footerService)
    {
        _footerService = footerService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _footerService.GetFooterAsync();
        return View(model);
    }
}
