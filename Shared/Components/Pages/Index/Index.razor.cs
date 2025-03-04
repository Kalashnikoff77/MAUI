using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Enums;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Shared.Components.Dialogs;

namespace Shared.Components.Pages.Index
{
    public partial class Index
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IRepository<GetAccountsRequestDto, GetAccountsResponseDto> _repoGetAccounts { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;

        List<AccountsViewDto> Accounts = null!;
        IDisposable? OnMessagesUpdatedHandler;

        protected override async Task OnInitializedAsync() =>
            await GetAccounts();

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
            {
                OnMessagesUpdatedHandler = OnMessagesUpdatedHandler.SignalRClient<OnMessagesUpdatedResponse>(CurrentState, async (response) =>
                {
                    await GetAccounts();
                    await InvokeAsync(StateHasChanged);
                });
            }
        }

        async Task GetAccounts()
        {
            var request = new GetAccountsRequestDto
            {
                IsRelationsIncluded = true,
                IsUsersIncluded = true,
                Order = EnumOrders.IdDesc,
                Take = 6
            };
            var response = await _repoGetAccounts.HttpPostAsync(request);
            Accounts = response.Response.Accounts;
        }

        public void Dispose() =>
            OnMessagesUpdatedHandler?.Dispose();
    }
}
