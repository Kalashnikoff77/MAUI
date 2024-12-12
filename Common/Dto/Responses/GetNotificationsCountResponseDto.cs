namespace Common.Dto.Responses
{
    public class GetNotificationsCountResponseDto : ResponseDtoBase
    {
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }
}
