namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for blog card component
/// </summary>
public class BlogCardViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string? FeaturedImage { get; set; }
    public string Url { get; set; } = "#";
    public DateTime PublishedDate { get; set; }
    public string? Author { get; set; }
    public string? AuthorImage { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public int ReadTimeMinutes { get; set; }
}

public class BlogSectionViewModel
{
    public string SectionTitle { get; set; } = "Latest News & Insights";
    public string? SectionSubtitle { get; set; }
    public List<BlogCardViewModel> Posts { get; set; } = new();
    public int ColumnsPerRow { get; set; } = 3;
    public string? ViewAllUrl { get; set; }
    public string ViewAllText { get; set; } = "View All Posts";
    public BlogLayout Layout { get; set; } = BlogLayout.Grid;
}

public enum BlogLayout
{
    Grid,
    List,
    Featured,
    Masonry
}
