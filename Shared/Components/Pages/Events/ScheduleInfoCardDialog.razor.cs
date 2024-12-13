using Microsoft.AspNetCore.Components;

namespace Shared.Components.Pages.Events
{
    public partial class ScheduleInfoCardDialog : ScheduleInfoBase, IDisposable
    {
        [Parameter, EditorRequired] public int ScheduleId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ScheduleForEventView = await GetScheduleForEvent(ScheduleId);
            scheduleDates = await GetScheduleDates(ScheduleForEventView.EventId);
            selectedSchedule = scheduleDates.First(s => s.Id == ScheduleForEventView.Id);   // Установим текущую дату мероприятия в выпадающем меню дат
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //OnScheduleChangedHandler = OnScheduleChangedHandler.SignalRClient<OnScheduleChangedResponse>(CurrentState, async (response) =>
            //{
            //    if (response.UpdatedSchedule != null)
            //    {
            //        ScheduleForEventView = response.UpdatedSchedule;
            //        await InvokeAsync(StateHasChanged);
            //    }
            //});
        }
    }
}
