using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetDiscussionsForEventsResponseDto : ResponseDtoBase
    {
        public int NumOfDiscussions { get; set; }
        public List<DiscussionsForEventsViewDto> Discussions { get; set; } = new List<DiscussionsForEventsViewDto>();
    }
}
