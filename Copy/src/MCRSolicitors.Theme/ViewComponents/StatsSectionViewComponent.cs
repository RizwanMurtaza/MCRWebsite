using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering statistics section
/// </summary>
public class StatsSectionViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(StatsSectionViewModel? model = null)
    {
        model ??= GetDefaultStatsModel();
        return View(model);
    }

    private static StatsSectionViewModel GetDefaultStatsModel()
    {
        return new StatsSectionViewModel
        {
            Stats = new List<StatItem>
            {
                new() { Value = "15", Suffix = "+", Label = "Years Experience" },
                new() { Value = "5000", Suffix = "+", Label = "Cases Handled" },
                new() { Value = "98", Suffix = "%", Label = "Client Satisfaction" },
                new() { Value = "24", Label = "Expert Solicitors" }
            },
            Layout = StatsLayout.Horizontal
        };
    }
}
