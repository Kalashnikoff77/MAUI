using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.State;

namespace Shared.Components.Pages.Account
{
    public partial class AccountInfoCardDialog : IDisposable
    {
        [CascadingParameter] CurrentState CurrentState { get; set; } = null!;
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;
        [Parameter, EditorRequired] public AccountsViewDto Account { get; set; } = null!;

        [Inject] IRepository<GetAccountsRequestDto, GetAccountsResponseDto> _repoGetAccount { get; set; } = null!;

        MudCarousel<PhotosForEventsDto> Carousel = null!;

        IDisposable? OnAccountDiscussionAddedHandler;

        protected override async Task OnInitializedAsync()
        {
            if (Account != null)
            {
                var apiResponse = await _repoGetAccount.HttpPostAsync(new GetAccountsRequestDto
                {
                    Id = Account.Id,
                    IsHobbiesIncluded = true,
                    IsPhotosIncluded = true,
                    IsUsersIncluded = true,
                    IsRelationsIncluded = true
                });
                Account = apiResponse.Response.Account;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //OnAccountDiscussionAddedHandler = OnAccountDiscussionAddedHandler.SignalRClient<OnScheduleChangedResponse>(CurrentState, async (response) =>
            //{
            //    var apiResponse = await _repoGetSchedules.HttpPostAsync(new GetSchedulesRequestDto { ScheduleId = response.ScheduleId });
            //    if (apiResponse.Response.Schedule != null)
            //    {
            //        //ScheduleForEventView = apiResponse.Response.Schedule;
            //        await InvokeAsync(StateHasChanged);
            //    }
            //});
        }

        public void Dispose() =>
            OnAccountDiscussionAddedHandler?.Dispose();
    }
}
