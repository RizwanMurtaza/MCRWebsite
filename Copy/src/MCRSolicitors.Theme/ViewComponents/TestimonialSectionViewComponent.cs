using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering testimonials section
/// </summary>
public class TestimonialSectionViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(TestimonialSectionViewModel? model = null)
    {
        model ??= GetDefaultTestimonialsModel();
        return View(model);
    }

    private static TestimonialSectionViewModel GetDefaultTestimonialsModel()
    {
        return new TestimonialSectionViewModel
        {
            SectionTitle = "What Our Clients Say",
            SectionSubtitle = "Trusted by Thousands Across Manchester",
            Testimonials = new List<TestimonialItem>
            {
                new()
                {
                    Quote = "MCR Solicitors handled my divorce case with incredible professionalism and sensitivity. They made a difficult time much easier to navigate.",
                    ClientName = "Sarah M.",
                    CaseType = "Family Law",
                    Rating = 5
                },
                new()
                {
                    Quote = "The immigration team helped me secure my visa when I thought all hope was lost. Forever grateful for their expertise and dedication.",
                    ClientName = "Ahmed K.",
                    CaseType = "Immigration",
                    Rating = 5
                },
                new()
                {
                    Quote = "Professional, efficient, and always kept me informed. My house purchase completed smoothly thanks to their conveyancing team.",
                    ClientName = "James T.",
                    CaseType = "Conveyancing",
                    Rating = 5
                }
            },
            Layout = TestimonialLayout.Carousel
        };
    }
}
