using Common.Dto.Requests;
using Common.Dto.Responses;
using Common.Dto.Views;
using Shared.Repository;
using Microsoft.AspNetCore.Components;
using Common.Extensions;

namespace Shared.Components.Pages
{
    public partial class Home : IDisposable
    {
        [Inject] IRepository<GetSchedulesRequestDto, GetSchedulesResponseDto> _repoGetSchedules { get; set; } = null!;
        [Inject] IRepository<GetFeaturesForEventsRequestDto, GetFeaturesForEventsResponseDto> _repoGetFeatures { get; set; } = null!;
        [Inject] IRepository<GetRegionsForEventsRequestDto, GetRegionsForEventsResponseDto> _repoGetRegions { get; set; } = null!;
        [Inject] IRepository<GetAdminsForEventsRequestDto, GetAdminsForEventsResponseDto> _repoGetAdmins { get; set; } = null!;

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

        bool IsButtonMoreVisible = false;
        bool IsNotFoundVisible = false;

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
            IsButtonMoreVisible = apiResponse.Response.Count <= SchedulesList.Count ? false : true;
            IsNotFoundVisible = SchedulesList.Count == 0 ? true : false;
        }

        public void Dispose() =>
            OnScheduleChangedHandler?.Dispose();
    }
}
