using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.Components.Dialogs;
using System.Text;

namespace Shared.Components.Pages.Events
{
    public partial class Events : IAsyncDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IRepository<GetSchedulesRequestDto, GetSchedulesResponseDto> _repoGetSchedules { get; set; } = null!;
        [Inject] IRepository<GetFeaturesForEventsRequestDto, GetFeaturesForEventsResponseDto> _repoGetFeatures { get; set; } = null!;
        [Inject] IRepository<GetRegionsForEventsRequestDto, GetRegionsForEventsResponseDto> _repoGetRegions { get; set; } = null!;
        [Inject] IRepository<GetAdminsForEventsRequestDto, GetAdminsForEventsResponseDto> _repoGetAdmins { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;
        [Inject] IJSProcessor _JSProcessor { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;
        [Inject] IComponentRenderer<OneSchedule> _renderer { get; set; } = null!;

        GetSchedulesRequestDto request = new GetSchedulesRequestDto { IsPhotosIncluded = true, Take = StaticData.SCHEDULES_PER_BLOCK };
        List<SchedulesForEventsViewDto> schedules = new List<SchedulesForEventsViewDto>();

        bool IsNotFoundVisible = false;

        DotNetObjectReference<Events> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;

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
                _JSModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", $"{CurrentState.WebUrl}/js/Pages/Events/EventsScroll.js");
                await _JSModule.InvokeVoidAsync("SetScrollEvent", _dotNetReference);
                await _JSProcessor.SetDotNetReference(_dotNetReference);

                OnScheduleChangedHandler = OnScheduleChangedHandler.SignalRClient(CurrentState, (Func<OnScheduleChangedResponse, Task>)(async (response) =>
                {
                    if (response.UpdatedSchedule != null)
                    {
                        // Есть ли в области видимости браузера такое расписание?
                        var index = schedules.FindIndex(i => i.Id == response.UpdatedSchedule.Id);
                        if (index >= 0)
                            schedules[index] = response.UpdatedSchedule;

                        await InvokeAsync(StateHasChanged);
                    }
                }));
            }
        }

        [JSInvokable]
        public async Task<string> GetNextSchedules()
        {
            var apiResponse = await _repoGetSchedules.HttpPostAsync(request);
            request.Skip = request.Skip + StaticData.SCHEDULES_PER_BLOCK;

            var html = new StringBuilder(5000);
            if (apiResponse.Response.Schedules != null)
            {
                schedules.AddRange(apiResponse.Response.Schedules);
                // Ручная генерация компонентов мероприятий
                foreach (var schedule in apiResponse.Response.Schedules)
                    html.Append(await _renderer.RenderAsync(new Dictionary<string, object?> { { "Schedule", schedule } }));
            }
            return html.ToString();
        }


        async Task LoadSchedulesAsync(bool toResetOffset = true)
        {
            StateHasChanged();

            if (toResetOffset)
            {
                //currentPage = 0;
                //request.Skip = currentPage * currentPageSize;
                //request.Take = currentPageSize;
            }

            var apiResponse = await _repoGetSchedules.HttpPostAsync(request);
            schedules.AddRange(apiResponse.Response.Schedules ?? new List<SchedulesForEventsViewDto>());
            IsNotFoundVisible = schedules.Count == 0 ? true : false;
        }


        [JSInvokable]
        public async Task ScheduleInfoCardDialog(int scheduleId)
        {
            var schedule = schedules.Find(x => x.Id == scheduleId);
            if (schedule != null)
                await ShowDialogs.ScheduleInfoCardDialogAsync(schedule);
        }

        [JSInvokable]
        public async Task AccountInfoCardDialog(int scheduleId)
        {
            var admin = schedules.Find(x => x.Id == scheduleId)?.Event?.Admin;
            if (admin != null)
                await ShowDialogs.AccountInfoCardDialogAsync(admin);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                OnScheduleChangedHandler?.Dispose();
                if (_JSModule != null)
                    await _JSModule.DisposeAsync();
                _dotNetReference?.Dispose();
            }
            // Отлов ошибки, когда соединение SignalR разорвано, но производится попытка вызвать JS. Возникает при перезагрузке страницы (F5)
            catch(JSDisconnectedException ex)
            {
            }
        }
    }
}
