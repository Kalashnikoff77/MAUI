using AutoMapper;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Enums;
using Data.Models;
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
        IMapper _mapper;

        public SignalRHub(IServiceProvider serviceProvider, Accounts connectedAccounts, IConfiguration configuration, IMapper mapper, ILogger<SignalRHub> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
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
                // Помечаем выбранные сообщения как прочитанные в БД
                if (request.MarkMessagesAsRead && request.MessagesIds != null && request.RecipientId != null)
                {
                    var service = _serviceProvider.GetService<IRepository<MarkMessagesAsReadRequestDto, ResponseDtoBase>>()!;
                    var apiResult = await service.HttpPostAsync(new MarkMessagesAsReadRequestDto
                    {
                        MarkAllMessagesAsRead = false,
                        SenderId = request.RecipientId.Value,
                        MessagesIds = request.MessagesIds,
                        Token = accountDetails.Token
                    });
                }

                // Помечаем все сообщения как прочитанные в БД
                if (request.MarkAllMessagesAsRead && request.MessageId != null && request.RecipientId != null)
                {
                    var service = _serviceProvider.GetService<IRepository<MarkMessagesAsReadRequestDto, ResponseDtoBase>>()!;
                    var apiResult = await service.HttpPostAsync(new MarkMessagesAsReadRequestDto 
                    {
                        MarkAllMessagesAsRead = true,
                        SenderId = request.RecipientId.Value,
                        Token = accountDetails.Token 
                    });
                }

                // Блокируем пользователя
                if (request.BlockAccount && request.RecipientId != null)
                {
                    var service = _serviceProvider.GetService<IRepository<AddMessageRequestDto, AddMessageResponseDto>>()!;
                    var apiResult = await service.HttpPostAsync(new AddMessageRequestDto
                    {
                        Type = EnumMessages.AccountBlocked,
                        RecipientId = request.RecipientId.Value,
                        Text = StaticData.NotificationTypes[EnumMessages.AccountBlocked].Text,
                        Token = accountDetails.Token
                    });
                }

                // Разблокируем пользователя
                if (request.UnblockAccount && request.RecipientId != null)
                {
                    var service = _serviceProvider.GetService<IRepository<AddMessageRequestDto, AddMessageResponseDto>>()!;
                    var apiResult = await service.HttpPostAsync(new AddMessageRequestDto
                    {
                        Type = EnumMessages.AccountUnblocked,
                        RecipientId = request.RecipientId.Value,
                        Text = StaticData.NotificationTypes[EnumMessages.AccountUnblocked].Text,
                        Token = accountDetails.Token
                    });
                }

                // Удаляем всю переписку
                if (request.DeleteMessages && request.RecipientId != null)
                {
                    var service = _serviceProvider.GetService<IRepository<AddMessageRequestDto, AddMessageResponseDto>>()!;
                    var apiResult = await service.HttpPostAsync(new AddMessageRequestDto
                    {
                        Type = EnumMessages.AllMessagesDeleted,
                        RecipientId = request.RecipientId.Value,
                        Text = StaticData.NotificationTypes[EnumMessages.AllMessagesDeleted].Text,
                        Token = accountDetails.Token
                    });
                }

                var response = _mapper.Map<OnMessagesUpdatedResponse>(request);

                await Clients.Caller.SendAsync(response.EnumSignalRHandlersClient.ToString(), response);

                if (request.RecipientId.HasValue)
                    await Clients.User(request.RecipientId.ToString()!).SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }
    }
}
