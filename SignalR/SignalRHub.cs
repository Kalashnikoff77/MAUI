using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Enums;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Models;

namespace SignalR
{
    public partial class SignalRHub : Hub
    {
        //public bool IsConnected => CurrentState.HubConnection.State == HubConnectionState.Connected;

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            var loggerScope = _logger.BeginScope("{@CurrentMethod}", nameof(OnConnectedAsync));
            _logger.LogInformation($"МЕТОД: {nameof(OnConnectedAsync)} (Id: {Context.UserIdentifier})");

            if (!string.IsNullOrWhiteSpace(Context.UserIdentifier))
            {
                Accounts.ConnectedAccounts.TryRemove(Context.UserIdentifier, out AccountDetails? value);

                // Сгенерим токен текущего пользователя
                int.TryParse(Context.User!.Claims.FirstOrDefault(x => x.Type == "Id")?.Value, out int accountId);
                Guid.TryParse(Context.User!.Claims.FirstOrDefault(x => x.Type == "Guid")?.Value, out Guid accountGuid);
                var token = StaticData.GenerateToken(accountId, accountGuid, _configuration);

                // Добавим пользователя в список онлайн пользователей
                Accounts.ConnectedAccounts.TryAdd(Context.UserIdentifier, new AccountDetails { Id = accountId, Token = token });

                // В БД отметим текущую дату и время входа на сайт текущего пользователя
                var service = _serviceProvider.GetService<IRepository<VisitsForAccountsUpdateRequestDto, ResponseDtoBase>>()!;
                await service.HttpPostAsync(new VisitsForAccountsUpdateRequestDto { Token = token });
            }

            // Отправим всем остальным пользователям актуальную информацию по залогиненным
            var model = new OnAccountConnectedResponse { ConnectedAccounts = Accounts.ConnectedAccounts.Select(s => s.Key) };
            await Clients.All.SendAsync(nameof(EnumSignalRHandlers.UpdateOnlineAccountsClient), model);

            // TODO Тест. Убрать (OK)
            var nulls = Accounts.ConnectedAccounts.Where(s => s.Key == "null").Count();
            if (nulls > 0)
                throw new Exception("Обнаружен null среди онлайн пользователей!");

            await base.OnConnectedAsync();
        }


        [Authorize]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var loggerScope = _logger.BeginScope("{@CurrentMethod}", nameof(OnDisconnectedAsync));
            _logger.LogInformation($"МЕТОД: {nameof(OnDisconnectedAsync)} (Id: {Context.UserIdentifier})");

            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                // В БД отметим текущую дату и время ухода с сайта текущего пользователя
                var service = _serviceProvider.GetService<IRepository<VisitsForAccountsUpdateRequestDto, ResponseDtoBase>>()!;
                await service.HttpPostAsync(new VisitsForAccountsUpdateRequestDto { Token = accountDetails.Token });
                // Удалим текущего пользователя из списка онлайн пользователей
                Accounts.ConnectedAccounts.Remove(Context.UserIdentifier!, out AccountDetails? value);
            }

            // Отправим всем остальным пользователям актуальную информацию по залогиненным
            var model = new OnAccountConnectedResponse { ConnectedAccounts = Accounts.ConnectedAccounts.Select(s => s.Key) };
            await Clients.All.SendAsync(nameof(EnumSignalRHandlers.UpdateOnlineAccountsClient), model);

            await base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// Получить AccountDetails из строкового Id в контексте запроса
        /// </summary>
        /// <returns>True - пользователь залогинен, false - не залогинен</returns>
        private bool GetAccountDetails(out AccountDetails accountDetails, string? userIdentifier)
        {
            accountDetails = null!;

            if (string.IsNullOrWhiteSpace(userIdentifier))
                return false;

            if (!Accounts.ConnectedAccounts.TryGetValue(userIdentifier, out AccountDetails? triedValue))
                return false;

            accountDetails = triedValue!;
            return true;
        }
    }
}
