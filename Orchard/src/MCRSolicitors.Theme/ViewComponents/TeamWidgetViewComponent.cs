using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class TeamWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new TeamWidgetViewModel
        {
            SectionTitle = GetTextField(widget, "TeamWidget", "SectionTitle") ?? "",
            SectionSubtitle = GetTextField(widget, "TeamWidget", "SectionSubtitle") ?? "",
            Members = new List<TeamMemberItemViewModel>()
        };

        // Get items from BagPart
        var bagItems = GetBagPartItems(widget, "BagPart");
        foreach (var item in bagItems)
        {
            model.Members.Add(new TeamMemberItemViewModel
            {
                Name = GetItemTextField(item, "TeamMemberItem", "Name") ?? "",
                Role = GetItemTextField(item, "TeamMemberItem", "Role") ?? "",
                Bio = GetItemTextField(item, "TeamMemberItem", "Bio") ?? "",
                Photo = GetItemMediaField(item, "TeamMemberItem", "Photo") ?? "",
                LinkedInUrl = GetItemTextField(item, "TeamMemberItem", "LinkedInUrl") ?? "",
                EmailAddress = GetItemTextField(item, "TeamMemberItem", "EmailAddress") ?? ""
            });
        }

        return View(model);
    }
}

public class TeamWidgetViewModel
{
    public string SectionTitle { get; set; } = "";
    public string SectionSubtitle { get; set; } = "";
    public List<TeamMemberItemViewModel> Members { get; set; } = new();
}

public class TeamMemberItemViewModel
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string Bio { get; set; } = "";
    public string Photo { get; set; } = "";
    public string LinkedInUrl { get; set; } = "";
    public string EmailAddress { get; set; } = "";
}
