using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetDiscussionsForEventsResponseDto : ResponseDtoBase
    {
        public List<DiscussionsForEventsViewDto> Discussions { get; set; } = new List<DiscussionsForEventsViewDto>();
    }
}
