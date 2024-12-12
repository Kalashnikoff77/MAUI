namespace Common.Dto.Requests
{
    public class GetMessagesRequestDto : RequestDtoBase
    {
        public override string Uri => "/Messages/Get";

        public int RecipientId { get; set; }

        public int? GetPreviousFromId { get; set; }
        public int? GetNextAfterId { get; set; }

        public bool MarkAsRead { get; set; } = false;
    }
}
