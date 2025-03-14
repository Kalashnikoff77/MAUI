using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        IServiceProvider _serviceProvider { get; set; } = null!;

        //public readonly string WebAPIUrl;
        //public readonly string SignalRUrl;
        public readonly string WebUrl;

        public CurrentState(IServiceProvider serviceProvider, IFormFactor formFactor, IJSProcessor JSProcessor, IJSRuntime JS, IConfiguration config, NavigationManager navigationManager)
        {
            _serviceProvider = serviceProvider;
            _formFactor = formFactor;
            _JSProcessor = JSProcessor;
            _config = config;
            _navigationManager = navigationManager;
            this.JS = JS;

            if (_formFactor.GetFormFactor() == "Phone")
            {
                //WebAPIUrl = _config.GetRequiredSection("WebAPI:AndroidHost").Value!;
                //SignalRUrl = _config.GetRequiredSection("SignalR:AndroidHost").Value!;
                WebUrl = _config.GetRequiredSection("Web:AndroidHost").Value!;
            }
            else
            {
                //WebAPIUrl = _config.GetRequiredSection("WebAPI:WinHost").Value!;
                //SignalRUrl = _config.GetRequiredSection("SignalR:WinHost").Value!;
                WebUrl = _config.GetRequiredSection("Web:WinHost").Value!;
            }
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
            var service = _serviceProvider.GetService<IRepository<AccountReloadRequestDto, AccountReloadResponseDto>>()!;
            var reloadResponse = await service.HttpPostAsync(new AccountReloadRequestDto() { Token = Account!.Token });
            if (reloadResponse.StatusCode == HttpStatusCode.OK)
            {
                reloadResponse.Response.Account.Token = Account.Token;
                SetAccount(reloadResponse.Response.Account);
            }
        }

        public async Task LogOutAsync()
        {
            // Снимает статус онлайн с аватаров текущего пользователя
            if (Account != null)
                ConnectedAccounts.Remove(Account.Id.ToString());

            await _formFactor.ClearLoginDataAsync();

            await _JSProcessor.UpdateOnlineAccountsClient(ConnectedAccounts);

            SetAccount(null);

            StateHasChanged();

            await SignalRConnect();

            await _JSProcessor.Redirect("/");
        }
    }
}
