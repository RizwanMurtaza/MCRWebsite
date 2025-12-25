using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using MCRSolicitors.Theme.ContentParts;
using MCRSolicitors.Theme.ViewModels;

namespace MCRSolicitors.Theme.Drivers;

/// <summary>
/// Display driver for StatItemPart - handles admin editing for stat items
/// </summary>
public class StatItemPartDisplayDriver : ContentPartDisplayDriver<StatItemPart>
{
    public override IDisplayResult Display(StatItemPart part, BuildPartDisplayContext context)
    {
        return Initialize<StatItemPartViewModel>("StatItemPart", model =>
        {
            model.Value = part.Value;
            model.Suffix = part.Suffix;
            model.Prefix = part.Prefix;
            model.Label = part.Label;
            model.IconClass = part.IconClass;
            model.DisplayOrder = part.DisplayOrder;
        }).Location("Detail", "Content:1");
    }

    public override IDisplayResult Edit(StatItemPart part, BuildPartEditorContext context)
    {
        return Initialize<StatItemPartViewModel>("StatItemPart_Edit", model =>
        {
            model.Value = part.Value;
            model.Suffix = part.Suffix;
            model.Prefix = part.Prefix;
            model.Label = part.Label;
            model.IconClass = part.IconClass;
            model.DisplayOrder = part.DisplayOrder;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(StatItemPart part, UpdatePartEditorContext context)
    {
        var viewModel = new StatItemPartViewModel();
        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Value = viewModel.Value;
        part.Suffix = viewModel.Suffix;
        part.Prefix = viewModel.Prefix;
        part.Label = viewModel.Label;
        part.IconClass = viewModel.IconClass;
        part.DisplayOrder = viewModel.DisplayOrder;

        return Edit(part, context);
    }
}
