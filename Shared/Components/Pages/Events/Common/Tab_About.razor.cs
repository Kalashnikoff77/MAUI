using Data.Dto.Views;
using Data.Services;
using Microsoft.AspNetCore.Components;
using Shared.Components.Dialogs;

namespace Shared.Components.Pages.Events.Common
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
