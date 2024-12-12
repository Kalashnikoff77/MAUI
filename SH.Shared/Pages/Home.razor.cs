using Common.Dto.Requests;
using Common.Dto.Responses;
using Common.Dto.Views;
using Common.Repository;
using Microsoft.AspNetCore.Components;
using SH.Shared.Services;

namespace SH.Shared.Pages
{
    public partial class Home
    {
        [Inject] IRepository<GetSchedulesRequestDto, GetSchedulesResponseDto> _repoGetSchedules { get; set; } = null!;

        GetSchedulesRequestDto request = new GetSchedulesRequestDto { IsPhotosIncluded = true };
        List<SchedulesForEventsViewDto> SchedulesList = new List<SchedulesForEventsViewDto>();

        protected override async Task OnInitializedAsync()
        {
            var apiResponse = await _repoGetSchedules.HttpPostAsync(request);
            SchedulesList.AddRange(apiResponse.Response.Schedules ?? new List<SchedulesForEventsViewDto>());

        }
    }
}
