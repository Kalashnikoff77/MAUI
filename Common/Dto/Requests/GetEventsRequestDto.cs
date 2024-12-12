namespace Common.Dto.Requests
{
    public class GetEventsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/Get";

        public int? EventId { get; set; }

        public bool IsPhotosIncluded { get; set; }
    }
}
