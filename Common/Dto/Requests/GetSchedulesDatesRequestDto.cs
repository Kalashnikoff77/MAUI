namespace Common.Dto.Requests
{
    public class GetSchedulesDatesRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/GetSchedulesDates";

        public int EventId { get; set; }
    }
}
