using Common.Dto.Views;
using Microsoft.AspNetCore.Components;
using Shared.Components.Dialogs;
using Shared.Services;

namespace Shared.Components.Pages.Events.Shared
{
    public partial class Tab_About
    {
        [Parameter, EditorRequired] public SchedulesForEventsViewDto ScheduleForEventView { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;
        [Inject] IJSProcessor _JSProcessor { get; set; } = null!;

        //protected override async Task OnAfterRenderAsync(bool firstRender) =>
        //    await _JSProcessor.ScrollToElement("top");
    }
}
