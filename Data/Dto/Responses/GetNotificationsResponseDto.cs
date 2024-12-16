using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetNotificationsResponseDto : ResponseDtoBase
    {
        public List<NotificationsViewDto>? Notifications { get; set; }
    }
}
