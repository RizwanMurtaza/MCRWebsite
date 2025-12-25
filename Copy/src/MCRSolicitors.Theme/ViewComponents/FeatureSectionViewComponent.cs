using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering why choose us / features section
/// </summary>
public class FeatureSectionViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(FeatureSectionViewModel? model = null)
    {
        model ??= GetDefaultFeaturesModel();
        return View(model);
    }

    private static FeatureSectionViewModel GetDefaultFeaturesModel()
    {
        return new FeatureSectionViewModel
        {
            SectionTitle = "Why Choose MCR Solicitors",
            SectionSubtitle = "Your Success Is Our Priority",
            Features = new List<FeatureItem>
            {
                new()
                {
                    Title = "Expert Legal Team",
                    Description = "Our solicitors have decades of combined experience across all areas of law.",
                    IconClass = "fas fa-user-tie"
                },
                new()
                {
                    Title = "Client-Focused Approach",
                    Description = "We listen to your needs and tailor our services to achieve the best outcomes.",
                    IconClass = "fas fa-handshake"
                },
                new()
                {
                    Title = "Transparent Pricing",
                    Description = "Clear, upfront pricing with no hidden fees. Know exactly what you're paying for.",
                    IconClass = "fas fa-pound-sign"
                },
                new()
                {
                    Title = "Fast Response Times",
                    Description = "We understand urgency. Get quick responses and timely updates on your case.",
                    IconClass = "fas fa-clock"
                }
            },
            Layout = FeatureLayout.Grid
        };
    }
}
