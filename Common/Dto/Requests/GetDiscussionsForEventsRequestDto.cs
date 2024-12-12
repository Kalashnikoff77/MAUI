namespace Common.Dto.Requests
{
    public class GetDiscussionsForEventsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/GetDiscussions";

        public int EventId { get; set; }

        public int? GetPreviousFromId { get; set; }
        public int? GetNextAfterId { get; set; }
    }
}
