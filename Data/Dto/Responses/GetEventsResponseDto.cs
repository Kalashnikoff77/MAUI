using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetEventsResponseDto : ResponseDtoBase
    {
        public EventsViewDto? Event { get; set; }

        public List<EventsViewDto>? Events { get; set; }
    }
}
