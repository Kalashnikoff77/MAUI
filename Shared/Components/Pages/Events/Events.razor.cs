using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Shared.Components.Dialogs;
using System.Text;

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
        [Inject] IServiceProvider _serviceProvider { get; set; } = null!;

        GetSchedulesRequestDto request = new GetSchedulesRequestDto { IsPhotosIncluded = true };
        List<SchedulesForEventsViewDto> schedules = new List<SchedulesForEventsViewDto>();

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
                jsModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", $"{CurrentState.WebUrl}/js/Pages/Events/EventsScroll.js");
                await jsModule.InvokeVoidAsync("SetScrollEvent", "Scroll", _dotNetReference);

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
            schedules.AddRange(apiResponse.Response.Schedules ?? new List<SchedulesForEventsViewDto>());

            // Ручная генерация компонентов событий
            return await RenderSchedules(apiResponse.Response.Schedules);

            //IsNotFoundVisible = SchedulesList.Count == 0 ? true : false;
            //request.Skip = ++currentPage * currentPageSize;
            //await LoadSchedulesAsync(false);
        }


        [JSInvokable]
        public static Task SuperTest(int scheduleId)
        {
            //var schedule = schedules.Find(x => x.Id == scheduleId);
            //ShowDialogs.ScheduleInfoCardDialogAsync(schedule);

            return Task.CompletedTask;
        }


        /// <summary>
        /// Ручная генерация компонента Schedule
        /// </summary>
        async Task<string> RenderSchedules(List<SchedulesForEventsViewDto> schedules)
        {
            await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _serviceProvider.GetRequiredService<ILoggerFactory>());
            var html = new StringBuilder(5000);
            foreach (var schedule in schedules)
            {
                html.Append(await htmlRenderer.Dispatcher.InvokeAsync(async () =>
                {
                    var dictionary = new Dictionary<string, object?>
                    {
                        { "Schedule", schedule }
                    };
                    var output = await htmlRenderer.RenderComponentAsync<OneSchedule>(ParameterView.FromDictionary(dictionary));
                    return output.ToHtmlString();
                }));
            }
            return html.ToString();
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
            schedules.AddRange(apiResponse.Response.Schedules ?? new List<SchedulesForEventsViewDto>());
            IsNotFoundVisible = schedules.Count == 0 ? true : false;
        }

        public void Dispose()
        {
            OnScheduleChangedHandler?.Dispose();
            _dotNetReference?.Dispose();
        }
    }
}
