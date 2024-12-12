using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetEventsResponseDto : ResponseDtoBase
    {
        public EventsViewDto? Event { get; set; }

        public List<EventsViewDto>? Events { get; set; }
    }
}
