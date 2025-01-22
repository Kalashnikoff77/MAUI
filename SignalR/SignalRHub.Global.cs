using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Models.SignalR;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;

namespace SignalR
{
    public partial class SignalRHub
    {
        /// <summary>
        /// Список онлайн пользователей
        /// </summary>
        Accounts Accounts { get; set; } = null!;

        IConfiguration _configuration;
        IServiceProvider _serviceProvider;
        ILogger<SignalRHub> _logger;

        public SignalRHub(IServiceProvider serviceProvider, Accounts connectedAccounts, IConfiguration configuration, ILogger<SignalRHub> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
            Accounts = connectedAccounts;
        }


        // Глобальный обработчик всех поступающих запросов
        [Authorize]
        public async Task GlobalHandler(SignalGlobalRequest request)
        {
            // Изменения в расписании мероприятия (пока только добавлено одно сообщение в обсуждение)
            if (request.OnScheduleChanged != null)
                await OnScheduleChangedAsync(request.OnScheduleChanged);

            // Пользователь изменил свой аватар
            if (request.OnAvatarChanged != null)
                await OnAvatarChangedAsync(request.OnAvatarChanged);

            // Обновить сообщения у двух пользователей (диалоговое окно страницы /messages)
            if (request.OnMessagesReload != null)
                await OnMessagesReloadAsync(request.OnMessagesReload);

            // Обновить список последних сообщений у двух пользователей (страница /messages)
            if (request.OnLastMessagesReload != null)
                await OnLastMessagesReloadAsync(request.OnLastMessagesReload);
        }


        /// <summary>
        /// Изменение в расписании мероприятия
        /// </summary>
        async Task OnScheduleChangedAsync(OnScheduleChanged request)
        {
            var service = _serviceProvider.GetService<IRepository<GetSchedulesRequestDto, GetSchedulesResponseDto>>()!;
            var apiResponse = await service.HttpPostAsync(new GetSchedulesRequestDto { ScheduleId = request.ScheduleId });
            var response = new OnScheduleChangedResponse { UpdatedSchedule = apiResponse.Response.Schedule };
            await Clients.All.SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
        }

        /// <summary>
        /// Пользователь изменил свой аватар, уведомляем всех об этом
        /// </summary>
        async Task OnAvatarChangedAsync(OnAvatarChanged request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnAvatarChangedResponse { NewAvatar = request.NewAvatar, AccountId = accountDetails.Id };
                await Clients.All.SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }

        /// <summary>
        /// Обновить сообщения у двух пользователей (диалоговое окно страницы /messages)
        /// </summary>
        async Task OnMessagesReloadAsync(OnMessagesReload request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnMessagesReloadResponse();
                await Clients
                    .Users([Context.UserIdentifier!, request.RecipientId?.ToString()!])
                    .SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }

        /// <summary>
        /// Обновить список последних сообщений у двух пользователей (страница /messages)
        /// </summary>
        async Task OnLastMessagesReloadAsync(OnLastMessagesReload request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnLastMessagesReloadResponse();
                await Clients
                    .Users([Context.UserIdentifier!, request.RecipientId?.ToString()!])
                    .SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }
    }
}
