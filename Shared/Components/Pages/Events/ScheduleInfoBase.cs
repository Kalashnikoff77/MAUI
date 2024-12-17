using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Data.State;

namespace Shared.Components.Pages.Events
{
    public class ScheduleInfoBase : ComponentBase
    {
        [CascadingParameter] protected CurrentState CurrentState { get; set; } = null!;
        [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;

        protected SchedulesForEventsViewDto ScheduleForEventView { get; set; } = null!;
        protected SchedulesDatesViewDto selectedSchedule { get; set; } = null!;
        protected IEnumerable<SchedulesDatesViewDto> scheduleDates { get; set; } = null!;

        [Inject] protected IRepository<GetSchedulesRequestDto, GetSchedulesResponseDto> _repoGetSchedules { get; set; } = null!;
        [Inject] protected IRepository<GetSchedulesDatesRequestDto, GetSchedulesDatesResponseDto> _repoGetSchedulesDates { get; set; } = null!;

        protected IDisposable? OnScheduleChangedHandler;

        protected virtual async Task ScheduleChangedAsync(SchedulesDatesViewDto schedule)
        {
            ScheduleForEventView = await GetScheduleForEvent(schedule.Id);
            selectedSchedule = scheduleDates.First(s => s.Id == ScheduleForEventView.Id);   // Установим текущую дату мероприятия в выпадающем меню дат
        }

        protected virtual async Task<SchedulesForEventsViewDto> GetScheduleForEvent(int scheduleId)
        {
            var response = await _repoGetSchedules.HttpPostAsync(new GetSchedulesRequestDto { ScheduleId = scheduleId });
            if (response.Response.Schedule == null)
                throw new Exception("Не найдено мероприятие!");
            return response.Response.Schedule;
        }

        protected virtual async Task<IEnumerable<SchedulesDatesViewDto>> GetScheduleDates(int eventId)
        {
            var responseSchedulesDates = await _repoGetSchedulesDates.HttpPostAsync(new GetSchedulesDatesRequestDto { EventId = eventId });
            if (responseSchedulesDates.Response.SchedulesDates == null)
                throw new Exception("Не найдено на одного расписания у мероприятия!");
            return responseSchedulesDates.Response.SchedulesDates;
        }

        public virtual void Dispose() =>
            OnScheduleChangedHandler?.Dispose();
    }
}
