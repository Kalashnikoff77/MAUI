using Common.Dto;
using Common.Dto.Requests;
using Common.Dto.Responses;
using Common.Dto.Views;
using Common.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Services;

namespace Shared.Components.Pages.Events.AddAndEdit
{
    public partial class AddScheduleForEventDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;
        [Parameter, EditorRequired] public EventsViewDto Event { get; set; } = null!;

        [Inject] IRepository<GetFeaturesRequestDto, GetFeaturesResponseDto> _repoGetFeatures { get; set; } = null!;

        public List<FeaturesDto> AllFeatures { get; set; } = new List<FeaturesDto>();

        SchedulesForEventsViewDto schedule { get; set; } = new SchedulesForEventsViewDto();

        List<SchedulesForEventsViewDto> schedules { get; set; } = new List<SchedulesForEventsViewDto>();

        protected async override Task OnInitializedAsync()
        {
            var featuresResponse = await _repoGetFeatures.HttpPostAsync(new GetFeaturesRequestDto());
            AllFeatures = featuresResponse.Response.Features;
        }

        const int maxStartDateDays = 30 * 3;
        const int maxEndDateDays = 30;
        bool isFormValid = false;
        string? errorMessage;

        bool _isOneTimeEvent = true;
        bool isOneTimeEvent
        {
            get => _isOneTimeEvent;
            set { _isOneTimeEvent = value; daysOfWeek.Clear(); CheckProperties(); }
        }

        // Дни недели
        HashSet<short> daysOfWeek = new HashSet<short>(7);

        DateTime? startDate
        {
            get => schedule.StartDate == DateTime.MinValue ? null : schedule.StartDate;
            set { schedule.StartDate = value!.Value; CheckProperties(); }
        }
        TimeSpan? _startTime;
        TimeSpan? startTime
        {
            get => _startTime;
            set { _startTime = value!.Value; CheckProperties(); }
        }

        DateTime? endDate
        {
            get => schedule.EndDate == DateTime.MinValue ? null : schedule.EndDate;
            set { schedule.EndDate = value!.Value; CheckProperties(); }
        }
        TimeSpan? _endTime;
        TimeSpan? endTime
        {
            get => _endTime;
            set { _endTime = value!.Value; CheckProperties(); }
        }

        void OnWeekChanged(short weekDay, bool isChecked)
        {
            if (isChecked)
                daysOfWeek.Add(weekDay);
            else
                daysOfWeek.Remove(weekDay);
            CheckProperties();
        }


        void OnFeatureChanged(FeaturesDto feature)
        {
            var index = schedule.Features!.FindIndex(x => x.Id == feature.Id);
            if (index >= 0)
                schedule.Features.RemoveAt(index);
            else
                schedule.Features.Add(feature);

            CheckProperties();
        }


        void CheckProperties()
        {
            isFormValid = false;
            errorMessage = null;

            if (schedule.Features!.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(schedule.Description) || schedule.Description.Length < StaticData.DB_EVENT_DESCRIPTION_MIN)
                return;

            if (startDate.HasValue && startTime.HasValue && endDate.HasValue && endTime.HasValue)
            {
                if (startDate.Value + startTime.Value >= endDate.Value + endTime.Value)
                {
                    errorMessage = "Дата начала мероприятия должна быть меньше даты его окончания";
                }
                else
                {
                    if (isOneTimeEvent)
                    {
                        schedules.Clear();
                        schedules.Add(new SchedulesForEventsViewDto
                        {
                            EventId = Event.Id,
                            Description = schedule.Description,
                            StartDate = startDate.Value + startTime.Value,
                            EndDate = endDate.Value + endTime.Value,
                            CostMan = schedule.CostMan,
                            CostWoman = schedule.CostWoman,
                            CostPair = schedule.CostPair,
                            Features = schedule.Features
                        });
                        isFormValid = true;
                    }
                    else
                    {
                        if (daysOfWeek.Count > 0)
                        {
                            var numOfSchedules = GetListOfSchedules(startDate.Value, startTime.Value, endDate.Value, endTime.Value, daysOfWeek);
                            if (numOfSchedules.Count > 0)
                                isFormValid = true;
                            else
                                errorMessage = $"В период с {startDate.Value.ToString("dd.MM.yyyy")} по {endDate.Value.ToString("dd.MM.yyyy")} ни одно мероприятие не попадает.";
                        }
                    }
                }
            }
            StateHasChanged();
        }


        void Submit()
        {
            // Финальная проверка перед закрытием окна
            CheckProperties();
            if (isFormValid)
                MudDialog.Close(DialogResult.Ok(schedules));
        }
        void Cancel() => MudDialog.Cancel();


        List<SchedulesForEventsViewDto> GetListOfSchedules(DateTime startDate, TimeSpan startTime, DateTime endDate, TimeSpan endTime, HashSet<short> daysOfWeek)
        {
            schedules.Clear();

            for (DateTime curDate = startDate; curDate <= endDate; curDate = curDate.AddDays(1))
            {
                if (daysOfWeek.Contains((short)curDate.DayOfWeek))
                {
                    schedules.Add(new SchedulesForEventsViewDto
                    {
                        EventId = Event.Id,
                        Description = schedule.Description,
                        StartDate = curDate + startTime,
                        EndDate = startTime > endTime ? curDate.AddDays(1) + endTime : curDate + endTime,
                        CostMan = schedule.CostMan,
                        CostWoman = schedule.CostWoman,
                        CostPair = schedule.CostPair,
                        Features = schedule.Features
                    });
                }
            }
            return schedules;
        }

        public Color DescriptionIconColor = Color.Default;
        public string? DescriptionValidator(string? text)
        {
            string? errorMessage = null;
            if (string.IsNullOrWhiteSpace(text) || text.Length < StaticData.DB_EVENT_DESCRIPTION_MIN)
                errorMessage = $"Введите не менее {StaticData.DB_EVENT_DESCRIPTION_MIN} символов";

            CheckProperties();
            return errorMessage;
        }
    }
}
