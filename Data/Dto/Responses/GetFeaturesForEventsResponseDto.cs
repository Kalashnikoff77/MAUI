using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetFeaturesForEventsResponseDto : ResponseDtoBase
    {
        public List<FeaturesForEventsViewDto> FeaturesForEvents { get; set; } = null!;
    }
}
