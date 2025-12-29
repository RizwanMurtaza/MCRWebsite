using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class TeamWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = widgetData != null ? UnwrapWidgetData(widgetData) : null;
        var model = new TeamWidgetViewModel
        {
            SectionTitle = widget != null ? (GetTextField(widget, "TeamWidget", "SectionTitle") ?? "Meet Our Team") : "Meet Our Team",
            SectionSubtitle = widget != null ? (GetTextField(widget, "TeamWidget", "SectionSubtitle") ?? "Experienced legal professionals dedicated to your success") : "Experienced legal professionals dedicated to your success",
            Members = new List<TeamMemberItemViewModel>()
        };

        // Get items from BagPart if widget exists
        if (widget != null)
        {
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
        }

        // Provide default team members if none exist
        if (model.Members.Count == 0)
        {
            model.Members = GetDefaultTeamMembers();
        }

        return View(model);
    }

    private List<TeamMemberItemViewModel> GetDefaultTeamMembers()
    {
        return new List<TeamMemberItemViewModel>
        {
            new TeamMemberItemViewModel
            {
                Name = "David Richardson",
                Role = "Managing Partner",
                Bio = "Over 25 years of experience in immigration and family law. David leads our team with a commitment to excellence and client care.",
                LinkedInUrl = "https://www.linkedin.com/company/mcrsolicitors",
                EmailAddress = "david@mcrsolicitors.co.uk"
            },
            new TeamMemberItemViewModel
            {
                Name = "Sarah Ahmed",
                Role = "Senior Immigration Solicitor",
                Bio = "Specialist in complex immigration cases including asylum, human rights, and business visas. Fluent in Urdu and Punjabi.",
                LinkedInUrl = "https://www.linkedin.com/company/mcrsolicitors",
                EmailAddress = "sarah@mcrsolicitors.co.uk"
            },
            new TeamMemberItemViewModel
            {
                Name = "James O'Connor",
                Role = "Family Law Partner",
                Bio = "Expert in divorce, child custody, and financial settlements. Known for his compassionate approach to sensitive family matters.",
                LinkedInUrl = "https://www.linkedin.com/company/mcrsolicitors",
                EmailAddress = "james@mcrsolicitors.co.uk"
            },
            new TeamMemberItemViewModel
            {
                Name = "Emma Williams",
                Role = "Personal Injury Solicitor",
                Bio = "Dedicated to securing fair compensation for accident victims. Handles workplace injuries, road accidents, and medical negligence claims.",
                LinkedInUrl = "https://www.linkedin.com/company/mcrsolicitors",
                EmailAddress = "emma@mcrsolicitors.co.uk"
            },
            new TeamMemberItemViewModel
            {
                Name = "Mohammed Ali",
                Role = "Immigration Caseworker",
                Bio = "Provides vital support on immigration applications. Multilingual with fluency in Arabic, Urdu, and Hindi.",
                LinkedInUrl = "https://www.linkedin.com/company/mcrsolicitors",
                EmailAddress = "mohammed@mcrsolicitors.co.uk"
            },
            new TeamMemberItemViewModel
            {
                Name = "Lisa Chen",
                Role = "Legal Secretary",
                Bio = "Ensures smooth case management and client communications. First point of contact for all enquiries.",
                LinkedInUrl = "https://www.linkedin.com/company/mcrsolicitors",
                EmailAddress = "lisa@mcrsolicitors.co.uk"
            }
        };
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
