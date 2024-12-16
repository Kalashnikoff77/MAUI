using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Shared.Components.Pages.Events
{
    public partial class EventInfo : ScheduleInfoBase, IDisposable
    {
        [Parameter, EditorRequired] public int EventId { get; set; }

        [Inject] IRepository<GetEventsRequestDto, GetEventsResponseDto> _repoGetEvent { get; set; } = null!;

        EventsViewDto Event { get; set; } = null!;
        
        protected override async Task OnInitializedAsync()
        {
            var response = await _repoGetEvent.HttpPostAsync(new GetEventsRequestDto { EventId = EventId, IsPhotosIncluded = true });
            if (response.Response.Event != null)
            {
                Event = response.Response.Event;
                scheduleDates = await GetScheduleDates(EventId);

                // Ищем ближайшее к текущей дате активное расписание мероприятия, если все мероприятия закончены, то берём первое из них
                selectedSchedule = scheduleDates.Where(x => x.StartDate > DateTime.Now).FirstOrDefault() ?? scheduleDates.First();
                ScheduleForEventView = await GetScheduleForEvent(selectedSchedule.Id);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //OnScheduleChangedHandler = OnScheduleChangedHandler.SignalRClient<OnScheduleChangedResponse>(CurrentState, async (response) =>
            //{
            //    if (response.UpdatedSchedule != null)
            //        ScheduleForEventView = response.UpdatedSchedule;
            //    await InvokeAsync(StateHasChanged);
            //});
        }
    }
}
