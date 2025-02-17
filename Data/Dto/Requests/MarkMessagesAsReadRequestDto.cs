namespace Data.Dto.Requests
{
    public class MarkMessagesAsReadRequestDto : RequestDtoBase
    {
        public override string Uri => "/messages/MarkAsRead";

        public int SenderId { get; set; }

        public IEnumerable<int>? MessagesIds { get; set; }

        public bool MarkAllMessagesAsRead { get; set; } = false;
    }
}
