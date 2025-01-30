using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetNotificationsResponseDto : ResponseDtoBase
    {
        public NotificationsDto? Notification { get; set; }
        public List<NotificationsDto>? Notifications { get; set; }

        /// <summary>
        /// Id отправителя и получателя уведомления
        /// </summary>
        public Dictionary<int, AccountsViewDto> Accounts { get; set; } = null!;
    }
}
