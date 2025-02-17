namespace Data.Dto.Requests
{
    public class MarkMessagesAsReadRequestDto : RequestDtoBase
    {
        public override string Uri => "/messages/MarkAsRead";

        public int? MessageId { get; set; }

        public bool MarkAllMessagesAsRead { get; set; } = false;
    }
}
