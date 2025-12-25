using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using MCRSolicitors.Theme.ContentParts;
using MCRSolicitors.Theme.ViewModels;

namespace MCRSolicitors.Theme.Drivers;

/// <summary>
/// Display driver for CtaPart - handles admin editing for Call-to-Action sections
/// </summary>
public class CtaPartDisplayDriver : ContentPartDisplayDriver<CtaPart>
{
    public override IDisplayResult Display(CtaPart part, BuildPartDisplayContext context)
    {
        return Initialize<CtaPartViewModel>("CtaPart", model =>
        {
            model.Subtitle = part.Subtitle;
            model.Title = part.Title;
            model.Description = part.Description;
            model.PrimaryButtonText = part.PrimaryButtonText;
            model.PrimaryButtonUrl = part.PrimaryButtonUrl;
            model.SecondaryButtonText = part.SecondaryButtonText;
            model.SecondaryButtonUrl = part.SecondaryButtonUrl;
            model.Style = part.Style;
            model.ShowContactInfo = part.ShowContactInfo;
            model.PhoneNumber = part.PhoneNumber;
            model.Email = part.Email;
        }).Location("Detail", "Content:1");
    }

    public override IDisplayResult Edit(CtaPart part, BuildPartEditorContext context)
    {
        return Initialize<CtaPartViewModel>("CtaPart_Edit", model =>
        {
            model.Subtitle = part.Subtitle;
            model.Title = part.Title;
            model.Description = part.Description;
            model.PrimaryButtonText = part.PrimaryButtonText;
            model.PrimaryButtonUrl = part.PrimaryButtonUrl;
            model.SecondaryButtonText = part.SecondaryButtonText;
            model.SecondaryButtonUrl = part.SecondaryButtonUrl;
            model.Style = part.Style;
            model.ShowContactInfo = part.ShowContactInfo;
            model.PhoneNumber = part.PhoneNumber;
            model.Email = part.Email;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(CtaPart part, UpdatePartEditorContext context)
    {
        var viewModel = new CtaPartViewModel();
        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Subtitle = viewModel.Subtitle;
        part.Title = viewModel.Title;
        part.Description = viewModel.Description;
        part.PrimaryButtonText = viewModel.PrimaryButtonText;
        part.PrimaryButtonUrl = viewModel.PrimaryButtonUrl;
        part.SecondaryButtonText = viewModel.SecondaryButtonText;
        part.SecondaryButtonUrl = viewModel.SecondaryButtonUrl;
        part.Style = viewModel.Style;
        part.ShowContactInfo = viewModel.ShowContactInfo;
        part.PhoneNumber = viewModel.PhoneNumber;
        part.Email = viewModel.Email;

        return Edit(part, context);
    }
}
