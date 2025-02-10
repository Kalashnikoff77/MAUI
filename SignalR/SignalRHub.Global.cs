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
            // Вызывается, когда нужно обновить состояние пользователя
            if (request.OnReloadAccountRequest != null)
                await OnReloadAccountAsync(request.OnReloadAccountRequest);

            // Изменения в расписании мероприятия (пока только добавлено одно сообщение в обсуждение)
            if (request.OnScheduleUpdatedRequest != null)
                await OnScheduleUpdatedAsync(request.OnScheduleUpdatedRequest);

            // Пользователь изменил свой аватар
            if (request.OnAvatarChangedRequest != null)
                await OnAvatarChangedAsync(request.OnAvatarChangedRequest);

            // Изменения в таблице Messages
            if (request.OnMessagesUpdatedRequest != null)
                await OnMessagesUpdatedAsync(request.OnMessagesUpdatedRequest);
        }


        /// <summary>
        /// Вызывается, когда нужно обновить состояние пользователя
        /// </summary>
        async Task OnReloadAccountAsync(OnReloadAccountRequest request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnReloadAccountResponse();

                await Clients.Caller.SendAsync(response.EnumSignalRHandlersClient.ToString(), response);

                if (request.AdditionalAccountId.HasValue)
                    await Clients.User(request.AdditionalAccountId.ToString()!).SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }

        /// <summary>
        /// Изменение в расписании мероприятия
        /// </summary>
        async Task OnScheduleUpdatedAsync(OnScheduleUpdatedRequest request)
        {
            var service = _serviceProvider.GetService<IRepository<GetSchedulesRequestDto, GetSchedulesResponseDto>>()!;
            var apiResponse = await service.HttpPostAsync(new GetSchedulesRequestDto { ScheduleId = request.ScheduleId });
            var response = new OnScheduleUpdatedResponse { UpdatedSchedule = apiResponse.Response.Schedule };
            await Clients.All.SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
        }

        /// <summary>
        /// Пользователь изменил свой аватар, уведомляем всех об этом
        /// </summary>
        async Task OnAvatarChangedAsync(OnAvatarChangedRequest request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnAvatarChangedResponse { NewAvatar = request.NewAvatar, AccountId = accountDetails.Id };
                await Clients.All.SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }

        /// <summary>
        /// Изменения в таблице Messages
        /// </summary>
        async Task OnMessagesUpdatedAsync(OnMessagesUpdatedRequest request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnMessagesUpdatedResponse
                {
                    MessageId = request.MessageId,
                    MessagesIds = request.MessagesIds,
                    AppendNewMessages = request.AppendNewMessages,
                    MarkMessagesAsRead = request.MarkMessagesAsRead,
                    UpdateMessage = request.UpdateMessage
                };

                await Clients.Caller.SendAsync(response.EnumSignalRHandlersClient.ToString(), response);

                if (request.RecipientId.HasValue)
                    await Clients.User(request.RecipientId.ToString()!).SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }
    }
}
