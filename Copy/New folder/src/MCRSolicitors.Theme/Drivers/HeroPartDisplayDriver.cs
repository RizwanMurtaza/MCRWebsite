using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using MCRSolicitors.Theme.ContentParts;
using MCRSolicitors.Theme.ViewModels;

namespace MCRSolicitors.Theme.Drivers;

/// <summary>
/// Display driver for HeroPart - handles both admin editing and frontend display
/// </summary>
public class HeroPartDisplayDriver : ContentPartDisplayDriver<HeroPart>
{
    public override IDisplayResult Display(HeroPart part, BuildPartDisplayContext context)
    {
        return Initialize<HeroPartViewModel>("HeroPart", model =>
        {
            model.Title = part.Title;
            model.Subtitle = part.Subtitle;
            model.Description = part.Description;
            model.PrimaryButtonText = part.PrimaryButtonText;
            model.PrimaryButtonUrl = part.PrimaryButtonUrl;
            model.SecondaryButtonText = part.SecondaryButtonText;
            model.SecondaryButtonUrl = part.SecondaryButtonUrl;
            model.BackgroundImageUrl = part.BackgroundImageUrl;
            model.ShowTrustIndicators = part.ShowTrustIndicators;
        }).Location("Detail", "Content:1");
    }

    public override IDisplayResult Edit(HeroPart part, BuildPartEditorContext context)
    {
        return Initialize<HeroPartViewModel>("HeroPart_Edit", model =>
        {
            model.Title = part.Title;
            model.Subtitle = part.Subtitle;
            model.Description = part.Description;
            model.PrimaryButtonText = part.PrimaryButtonText;
            model.PrimaryButtonUrl = part.PrimaryButtonUrl;
            model.SecondaryButtonText = part.SecondaryButtonText;
            model.SecondaryButtonUrl = part.SecondaryButtonUrl;
            model.BackgroundImageUrl = part.BackgroundImageUrl;
            model.ShowTrustIndicators = part.ShowTrustIndicators;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(HeroPart part, UpdatePartEditorContext context)
    {
        var viewModel = new HeroPartViewModel();
        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Title = viewModel.Title;
        part.Subtitle = viewModel.Subtitle;
        part.Description = viewModel.Description;
        part.PrimaryButtonText = viewModel.PrimaryButtonText;
        part.PrimaryButtonUrl = viewModel.PrimaryButtonUrl;
        part.SecondaryButtonText = viewModel.SecondaryButtonText;
        part.SecondaryButtonUrl = viewModel.SecondaryButtonUrl;
        part.BackgroundImageUrl = viewModel.BackgroundImageUrl;
        part.ShowTrustIndicators = viewModel.ShowTrustIndicators;

        return Edit(part, context);
    }
}
