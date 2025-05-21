using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Components.Dialogs;

namespace Shared.Components.Pages.Account
{
    public partial class AccountInfoCardDialog : IDisposable
    {
        [CascadingParameter] CurrentState CurrentState { get; set; } = null!;
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;

        [Parameter, EditorRequired] public AccountsViewDto Account { get; set; } = null!;

        [Inject] IRepository<GetAccountsRequestDto, GetAccountsResponseDto> _repoGetAccount { get; set; } = null!;

        MudCarousel<PhotosForEventsDto> Carousel = null!;

        IDisposable? OnAccountDiscussionAddedHandler;
        IDisposable? OnMessagesUpdatedHandler;

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
            if (!firstRender)
            {
                OnMessagesUpdatedHandler = OnMessagesUpdatedHandler.SignalRClient<OnMessagesUpdatedResponse>(CurrentState, async (response) =>
                {
                    await InvokeAsync(StateHasChanged);
                });
            }

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
