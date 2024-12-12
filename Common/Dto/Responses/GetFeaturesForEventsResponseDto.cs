using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetFeaturesForEventsResponseDto : ResponseDtoBase
    {
        public List<FeaturesForEventsViewDto> FeaturesForEvents { get; set; } = null!;
    }
}
