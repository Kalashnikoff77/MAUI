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

            // Пользователь отправил сообщение другому пользователю
            if (request.OnMessageAdded != null)
                await OnMessageAddedAsync(request.OnMessageAdded);
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
        /// Пользователь отправил сообщение другому пользователю
        /// </summary>
        async Task OnMessageAddedAsync(OnMessageAdded request)
        {
            if (GetAccountDetails(out AccountDetails accountDetails, Context.UserIdentifier))
            {
                var response = new OnMessageAddedResponse { Message = request.Message };
                await Clients
                    .Users([Context.UserIdentifier!, request.Message.RecipientId.ToString()])
                    .SendAsync(response.EnumSignalRHandlersClient.ToString(), response);
            }
        }
    }
}
