using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering hero sections
/// Single Responsibility: Only handles hero section rendering
/// </summary>
public class HeroViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(HeroViewModel? model = null)
    {
        model ??= GetDefaultHeroModel();
        return View(model);
    }

    private static HeroViewModel GetDefaultHeroModel()
    {
        return new HeroViewModel
        {
            Title = "Expert Legal Services in Manchester",
            Subtitle = "Your Trusted Legal Partner",
            Description = "MCR Solicitors provides comprehensive legal solutions tailored to your needs. With years of experience and a dedicated team, we're here to help you navigate any legal challenge.",
            PrimaryButton = new HeroCtaButton
            {
                Text = "Free Consultation",
                Url = "/contact",
                Style = ButtonStyle.Primary
            },
            SecondaryButton = new HeroCtaButton
            {
                Text = "Our Services",
                Url = "/services",
                Style = ButtonStyle.Outline
            },
            ShowTrustIndicators = true,
            TrustIndicators = new List<TrustIndicator>
            {
                new() { Icon = "fas fa-shield-alt", Text = "SRA Regulated" },
                new() { Icon = "fas fa-award", Text = "Lexcel Accredited" },
                new() { Icon = "fas fa-users", Text = "1000+ Happy Clients" }
            }
        };
    }
}
