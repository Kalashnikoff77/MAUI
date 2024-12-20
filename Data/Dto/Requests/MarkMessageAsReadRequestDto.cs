namespace Data.Dto.Requests
{
    public class MarkMessageAsReadRequestDto : RequestDtoBase
    {
        public override string Uri => "/messages/MarkAsRead";

        public int? MessageId { get; set; }

        public bool MarkAllAsRead { get; set; } = false;
    }
}
