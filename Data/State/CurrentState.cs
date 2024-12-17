using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System.Net;

namespace Data.State
{
    public partial class CurrentState
    {
        IFormFactor _formFactor { get; set; }
        IJSProcessor _JSProcessor { get; set; } = null!;
        IConfiguration _config { get; set; } = null!;
        NavigationManager _navigationManager { get; set; } = null!;

        public CurrentState(IFormFactor formFactor, IJSProcessor JSProcessor, IJSRuntime JS, IConfiguration config, NavigationManager navigationManager)
        {
            _formFactor = formFactor;
            _JSProcessor = JSProcessor;
            _config = config;
            _navigationManager = navigationManager;
            this.JS = JS;
        }

        /// <summary>
        /// Данные о залогиненном пользователе
        /// </summary>
        public AccountsViewDto? Account { get; set; } = null;

        /// <summary>
        /// Пользователь залогинен? (для быстрой проверки)
        /// </summary>
        public bool IsAccountLoggedIn { get; set; }

        public event Action? OnChange;
        public IJSRuntime JS { get; set; } = null!;

        IRepository<AccountReloadRequestDto, AccountReloadResponseDto> _repoReload { get; set; } = null!;

        /// <summary>
        /// Вызывает StateHasChanged по всему сайту
        /// </summary>
        public void StateHasChanged() => OnChange?.Invoke();

        /// <summary>
        /// Внесём данные о полученном аккаунте в CurrentState.Account
        /// </summary>
        public void SetAccount(AccountsViewDto? loggedInAccount)
        {
            Account = loggedInAccount;
            IsAccountLoggedIn = loggedInAccount == null ? false : true;
        }


        /// <summary>
        /// Загружает новые данные о пользователе в CurrentState.Account из базы данных
        /// </summary>
        public async Task ReloadAccountAsync()
        {
            var reloadResponse = await _repoReload.HttpPostAsync(new AccountReloadRequestDto() { Token = Account!.Token });

            if (reloadResponse.StatusCode == HttpStatusCode.OK)
            {
                reloadResponse.Response.Account.Token = Account.Token;
                SetAccount(reloadResponse.Response.Account);
            }
        }

        public async Task LogOutAsync()
        {
            // Снимает статус онлайн с аватаров текущего пользователя
            //if (Account != null)
            //    ConnectedAccounts.Remove(Account.Id.ToString());

            await _formFactor.ClearLoginDataAsync();

            //await _JSProcessor.UpdateOnlineAccountsClient(ConnectedAccounts);

            SetAccount(null);

            StateHasChanged();

            //await SignalRConnect();

            await _JSProcessor.Redirect("/");
        }
    }
}
