using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Shared.State;

namespace Shared.Components.Layout
{
    public partial class MainLayout : IAsyncDisposable
    {
        [Inject] IFormFactor _formFactor { get; set; } = null!;
        [Inject] CurrentState CurrentState { get; set; } = null!;
        [Inject] IConfiguration _configuration { get; set; } = null!;

        [Inject] IRepository<LoginRequestDto, LoginResponseDto> _repoLogin { get; set; } = null!;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var loginData = await _formFactor.GetLoginDataAsync();

                if (loginData != null)
                {
                    var apiResponse = await _repoLogin.HttpPostAsync(loginData);
                    if (apiResponse.Response.Account != null)
                    {
                        apiResponse.Response.Account!.Token = StaticData.GenerateToken(apiResponse.Response.Account.Id, apiResponse.Response.Account.Guid, _configuration);
                        CurrentState.SetAccount(apiResponse.Response.Account);
                        CurrentState.StateHasChanged();
                    }
                    // Если пользователя из базы удалили, но он залогинен, то сделаем принудительный выход
                    else
                    {
                        await CurrentState.LogOutAsync();
                    }
                }

                //await CurrentState.SignalRConnect();
            }
        }

        public async ValueTask DisposeAsync()
        {
            CurrentState.OnChange -= StateHasChanged;
            //await CurrentState.SignalRDisconnect();
        }
    }
}
