using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetRegionsForEventsResponseDto : ResponseDtoBase
    {
        public List<RegionsForEventsViewDto> RegionsForEvents { get; set; } = null!;
    }
}
