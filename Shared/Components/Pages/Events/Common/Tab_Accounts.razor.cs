using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Services;
using Microsoft.AspNetCore.Components;
using Shared.Components.Dialogs;

namespace Shared.Components.Pages.Events.Common
{
    public partial class Tab_Accounts
    {
        [Parameter, EditorRequired] public SchedulesForEventsViewDto ScheduleForEventView { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;

        [Inject] IRepository<GetSchedulesForAccountsRequestDto, GetSchedulesForAccountsResponseDto> _repoGetSchedulesForAccounts { get; set; } = null!;

        IEnumerable<SchedulesForAccountsViewDto> Schedules { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var response = await _repoGetSchedulesForAccounts.HttpPostAsync(new GetSchedulesForAccountsRequestDto { ScheduleId = ScheduleForEventView.Id });
            Schedules = response.Response.Accounts;
        }
    }
}
