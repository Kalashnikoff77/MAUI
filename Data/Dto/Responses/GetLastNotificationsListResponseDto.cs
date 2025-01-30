using Data.Dto.Sp;

namespace Data.Dto.Responses
{
    public class GetLastNotificationsListResponseDto : ResponseDtoBase
    {
        public List<LastNotificationsForAccountSpDto>? LastNotificationsList { get; set; }
    }
}
