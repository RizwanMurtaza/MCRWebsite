using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering blog section
/// </summary>
public class BlogSectionViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(BlogSectionViewModel? model = null)
    {
        model ??= GetDefaultBlogModel();
        return View(model);
    }

    private static BlogSectionViewModel GetDefaultBlogModel()
    {
        return new BlogSectionViewModel
        {
            SectionTitle = "Latest News & Insights",
            SectionSubtitle = "Stay Informed with Our Legal Blog",
            Posts = new List<BlogCardViewModel>
            {
                new()
                {
                    Title = "Understanding Your Rights in a Divorce",
                    Excerpt = "Navigate the complexities of divorce proceedings with our comprehensive guide to your legal rights.",
                    Category = "Family Law",
                    PublishedDate = DateTime.Now.AddDays(-3),
                    ReadTimeMinutes = 5,
                    Url = "/blog/divorce-rights"
                },
                new()
                {
                    Title = "New Immigration Rules for 2024",
                    Excerpt = "Important updates to UK immigration policies that could affect your visa application.",
                    Category = "Immigration",
                    PublishedDate = DateTime.Now.AddDays(-7),
                    ReadTimeMinutes = 4,
                    Url = "/blog/immigration-rules-2024"
                },
                new()
                {
                    Title = "First-Time Buyer's Legal Checklist",
                    Excerpt = "Everything you need to know about the legal aspects of buying your first home.",
                    Category = "Conveyancing",
                    PublishedDate = DateTime.Now.AddDays(-14),
                    ReadTimeMinutes = 6,
                    Url = "/blog/first-time-buyer-checklist"
                }
            },
            ViewAllUrl = "/blog",
            ViewAllText = "View All Articles"
        };
    }
}
