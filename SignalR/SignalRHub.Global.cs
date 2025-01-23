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

            // Обновить сообщения у двух пользователей в диалоговом окне страницы /messages
            if (request.OnGetNewMessages != null)
                await OnGetNewMessagesAsync(request.OnGetNewMessages);

            // Обновить сообщения у пользователя в диалоговом окне страницы /messages (например: отметка о прочтении, изменение текста)
            if (request.OnMarkMessagesAsRead != null)
                await OnMarkMessagesAsReadAsync(request.OnMarkMessagesAsRead);

            // Обновить список последних сообщений у двух пользователей (страница /messages)
            if (request.OnUpdateLastMessages != null)
                await OnUpdateLastMessagesAsync(request.OnUpdateLastMessages);
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
        /// Добавим сообщения в диалог двух пользователей (диалоговое окно страницы /messages)
        /// </summary>
        async Task OnGetNewMessagesAsync(OnGetNewMessages request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnGetNewMessagesResponse();
                await Clients
                    .Users([Context.UserIdentifier!, request.RecipientId?.ToString()!])
                    .SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }

        /// <summary>
        /// Пометим сообщения как прочитанные в MessageDialog страницы /messages
        /// </summary>
        async Task OnMarkMessagesAsReadAsync(OnMarkMessagesAsRead request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnMarkMessagesAsReadResponse { Messages = request.Messages };
                await Clients
                    .User(request.RecipientId.ToString())
                    .SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }

        /// <summary>
        /// Обновим список последних сообщений у двух пользователей (страница /messages)
        /// </summary>
        async Task OnUpdateLastMessagesAsync(OnUpdateLastMessages request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnUpdateLastMessagesResponse();
                await Clients
                    .Users([Context.UserIdentifier!, request.RecipientId.ToString()!])
                    .SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }
    }
}
