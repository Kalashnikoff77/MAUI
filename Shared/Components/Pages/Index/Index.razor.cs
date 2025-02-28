using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Enums;
using Data.Services;
using Microsoft.AspNetCore.Components;

namespace Shared.Components.Pages.Index
{
    public partial class Index
    {
        [Inject] IRepository<GetAccountsRequestDto, GetAccountsResponseDto> _repoGetAccounts { get; set; } = null!;

        List<AccountsViewDto> Accounts = null!;

        protected override async Task OnInitializedAsync()
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

    }
}
