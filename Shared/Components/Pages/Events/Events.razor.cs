using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.Components.Pages.Messages;

namespace Shared.Components.Pages.Events
{
    public partial class Events : IDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IRepository<GetSchedulesRequestDto, GetSchedulesResponseDto> _repoGetSchedules { get; set; } = null!;
        [Inject] IRepository<GetFeaturesForEventsRequestDto, GetFeaturesForEventsResponseDto> _repoGetFeatures { get; set; } = null!;
        [Inject] IRepository<GetRegionsForEventsRequestDto, GetRegionsForEventsResponseDto> _repoGetRegions { get; set; } = null!;
        [Inject] IRepository<GetAdminsForEventsRequestDto, GetAdminsForEventsResponseDto> _repoGetAdmins { get; set; } = null!;
        [Inject] IJSProcessor _JSProcessor { get; set; } = null!;

        GetSchedulesRequestDto request = new GetSchedulesRequestDto { IsPhotosIncluded = true };
        List<SchedulesForEventsViewDto> SchedulesList = new List<SchedulesForEventsViewDto>();

        /// <summary>
        /// Для предотвращения повторного выполнения OnParametersSet (выполняется при переходе на другую ссылку)
        /// </summary>
        protected bool isFirstSetParameters = true;

        // Текущая отображаемая страница с мероприятиями
        int currentPage = 0;
        // Текущее кол-во отображаемых мероприятий на странице
        const int currentPageSize = 10;
        bool IsNotFoundVisible = false;

        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;
        DotNetObjectReference<Events> _dotNetReference { get; set; } = null!;
        IJSObjectReference jsModule { get; set; } = null!;

        IDisposable? OnScheduleChangedHandler;

        protected override async Task OnInitializedAsync()
        {
            var featuresResponse = await _repoGetFeatures.HttpPostAsync(new GetFeaturesForEventsRequestDto());
            FeaturesList = featuresResponse.Response.FeaturesForEvents;

            var adminsResponse = await _repoGetAdmins.HttpPostAsync(new GetAdminsForEventsRequestDto());
            AdminsList = adminsResponse.Response.AdminsForEvents;

            var regionsResponse = await _repoGetRegions.HttpPostAsync(new GetRegionsForEventsRequestDto());
            RegionsList = regionsResponse.Response.RegionsForEvents;

            await LoadSchedulesAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dotNetReference = DotNetObjectReference.Create(this);
                jsModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/Pages/Events/EventsScroll.js");
                await jsModule.InvokeVoidAsync("SetScrollEvent", "Body", _dotNetReference);

                OnScheduleChangedHandler = OnScheduleChangedHandler.SignalRClient(CurrentState, (Func<OnScheduleChangedResponse, Task>)(async (response) =>
                {
                    if (response.UpdatedSchedule != null)
                    {
                        // Есть ли в области видимости браузера такое расписание?
                        var index = SchedulesList.FindIndex(i => i.Id == response.UpdatedSchedule.Id);
                        if (index >= 0)
                            SchedulesList[index] = response.UpdatedSchedule;

                        await InvokeAsync(StateHasChanged);
                    }
                }));
            }
        }


        async Task MoreSchedulesAsync()
        {
            request.Skip = ++currentPage * currentPageSize;
            await LoadSchedulesAsync(false);
        }

        async Task LoadSchedulesAsync(bool toResetOffset = true)
        {
            StateHasChanged();

            if (toResetOffset)
            {
                currentPage = 0;
                request.Skip = currentPage * currentPageSize;
                request.Take = currentPageSize;
            }

            var apiResponse = await _repoGetSchedules.HttpPostAsync(request);
            SchedulesList.AddRange(apiResponse.Response.Schedules ?? new List<SchedulesForEventsViewDto>());
            IsNotFoundVisible = SchedulesList.Count == 0 ? true : false;
        }

        public void Dispose()
        {
            OnScheduleChangedHandler?.Dispose();
            _dotNetReference?.Dispose();
        }
    }
}
