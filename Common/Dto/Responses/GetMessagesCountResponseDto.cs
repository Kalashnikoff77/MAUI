namespace Common.Dto.Responses
{
    public class GetMessagesCountResponseDto : ResponseDtoBase
    {
        public int TotalCount { get; set; }

        public int UnreadCount { get; set; }
    }
}
