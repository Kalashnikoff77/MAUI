using Common.Dto;
using Common.Dto.Requests;
using Common.Dto.Responses;
using Common.Dto.Views;
using Common.Extensions;
using Common.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Services;

namespace Shared.Components.Pages.Events.AddAndEdit
{
    public partial class EditScheduleForEventDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;
        [Parameter, EditorRequired] public SchedulesForEventsViewDto Schedule { get; set; } = null!;

        [Inject] IRepository<GetFeaturesRequestDto, GetFeaturesResponseDto> _repoGetFeatures { get; set; } = null!;
        List<FeaturesDto> AllFeatures { get; set; } = new List<FeaturesDto>();

        SchedulesForEventsViewDto ScheduleCopy { get; set; } = new SchedulesForEventsViewDto();

        const int maxStartDateDays = 30 * 3;
        const int maxEndDateDays = 30;
        bool isFormValid = false;
        string? errorMessage;

        protected async override Task OnInitializedAsync()
        {
            ScheduleCopy = Schedule.DeepCopy<SchedulesForEventsViewDto>()!;

            if (ScheduleCopy.Features == null)
                ScheduleCopy.Features = new List<FeaturesDto>();

            startTime = new TimeSpan(ScheduleCopy.StartDate.Hour, ScheduleCopy.StartDate.Minute, ScheduleCopy.StartDate.Second);
            endTime = new TimeSpan(ScheduleCopy.EndDate.Hour, ScheduleCopy.EndDate.Minute, ScheduleCopy.EndDate.Second);

            var featuresResponse = await _repoGetFeatures.HttpPostAsync(new GetFeaturesRequestDto());
            AllFeatures = featuresResponse.Response.Features;
        }

        DateTime? startDate
        {
            get => ScheduleCopy.StartDate == DateTime.MinValue ? null : ScheduleCopy.StartDate;
            set { ScheduleCopy.StartDate = value!.Value; CheckProperties(); }
        }
        TimeSpan? _startTime;
        TimeSpan? startTime
        {
            get => _startTime;
            set { _startTime = value!.Value; CheckProperties(); }
        }

        DateTime? endDate
        {
            get => ScheduleCopy.EndDate == DateTime.MinValue ? null : ScheduleCopy.EndDate;
            set { ScheduleCopy.EndDate = value!.Value; CheckProperties(); }
        }
        TimeSpan? _endTime;
        TimeSpan? endTime
        {
            get => _endTime;
            set { _endTime = value!.Value; CheckProperties(); }
        }

        void OnFeatureChanged(FeaturesDto feature)
        {
            var index = ScheduleCopy.Features!.FindIndex(x => x.Id == feature.Id);
            if (index >= 0)
                ScheduleCopy.Features.RemoveAt(index);
            else
                ScheduleCopy.Features.Add(feature);
            CheckProperties();
        }

        void CheckProperties()
        {
            isFormValid = false;
            errorMessage = null;

            if (ScheduleCopy.Features!.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(ScheduleCopy.Description) || ScheduleCopy.Description.Length < StaticData.DB_EVENT_DESCRIPTION_MIN)
                return;

            if (startDate.HasValue && startTime.HasValue && endDate.HasValue && endTime.HasValue)
            {
                if (startDate.Value.Date + startTime.Value >= endDate.Value.Date + endTime.Value)
                    errorMessage = "Дата начала мероприятия должна быть меньше даты его окончания";
                else
                    isFormValid = true;
            }

            StateHasChanged();
        }


        void Submit()
        {
            // Финальная проверка перед закрытием окна
            CheckProperties();
            if (isFormValid)
                MudDialog.Close(DialogResult.Ok(ScheduleCopy));
        }
        void Cancel() => MudDialog.Cancel();


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
