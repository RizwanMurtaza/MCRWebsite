using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class TeamSectionWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["TeamSectionWidget"];
        var teamMembersBag = contentItem?.Content["BagPart"]?["ContentItems"];

        var model = new TeamSectionWidgetViewModel
        {
            SectionTitle = GetDynamicValue(GetDynamicValue(part, "SectionTitle"), "Text")?.ToString(),
            SectionSubtitle = GetDynamicValue(GetDynamicValue(part, "SectionSubtitle"), "Text")?.ToString(),
            CssClass = GetDynamicValue(GetDynamicValue(part, "CssClass"), "Text")?.ToString(),
            TeamMembers = new List<TeamMemberViewModel>()
        };

        if (teamMembersBag != null)
        {
            foreach (var memberItem in teamMembersBag)
            {
                var memberPart = GetDynamicValue(memberItem, "TeamMember");
                if (memberPart != null)
                {
                    model.TeamMembers.Add(new TeamMemberViewModel
                    {
                        Name = GetDynamicValue(GetDynamicValue(memberPart, "Name"), "Text")?.ToString(),
                        Role = GetDynamicValue(GetDynamicValue(memberPart, "Role"), "Text")?.ToString(),
                        Bio = GetDynamicValue(GetDynamicValue(memberPart, "Bio"), "Text")?.ToString(),
                        Photo = GetMediaPath(GetDynamicValue(memberPart, "Photo")),
                        LinkedInUrl = GetDynamicValue(GetDynamicValue(memberPart, "LinkedInUrl"), "Text")?.ToString(),
                        EmailAddress = GetDynamicValue(GetDynamicValue(memberPart, "EmailAddress"), "Text")?.ToString(),
                        DisplayOrder = GetIntValue(GetDynamicValue(GetDynamicValue(memberPart, "DisplayOrder"), "Value"), 1)
                    });
                }
            }
            model.TeamMembers = model.TeamMembers.OrderBy(m => m.DisplayOrder).ToList();
        }

        return View(model);
    }

    private static dynamic? GetDynamicValue(dynamic? obj, string name)
    {
        if (obj == null) return null;
        try
        {
            return obj[name];
        }
        catch
        {
            return null;
        }
    }

    private string? GetMediaPath(dynamic? mediaField)
    {
        if (mediaField == null) return null;
        try
        {
            var paths = GetDynamicValue(mediaField, "Paths");
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    if (path != null)
                    {
                        return "/media/" + path.ToString();
                    }
                }
            }
        }
        catch { }
        return null;
    }

    private int GetIntValue(dynamic? value, int defaultValue)
    {
        if (value == null) return defaultValue;
        try
        {
            if (value is int intVal) return intVal;
            if (value is decimal decVal) return (int)decVal;
            if (value is double dblVal) return (int)dblVal;
            if (int.TryParse(value.ToString(), out int result)) return result;
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }
}

public class TeamSectionWidgetViewModel
{
    public string? SectionTitle { get; set; }
    public string? SectionSubtitle { get; set; }
    public string? CssClass { get; set; }
    public List<TeamMemberViewModel> TeamMembers { get; set; } = new();
}

public class TeamMemberViewModel
{
    public string? Name { get; set; }
    public string? Role { get; set; }
    public string? Bio { get; set; }
    public string? Photo { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? EmailAddress { get; set; }
    public int DisplayOrder { get; set; } = 1;
}
