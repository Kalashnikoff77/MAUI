using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetRegionsForEventsResponseDto : ResponseDtoBase
    {
        public List<RegionsForEventsViewDto> RegionsForEvents { get; set; } = null!;
    }
}
