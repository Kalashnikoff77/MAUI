using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetAdminsForEventsResponseDto : ResponseDtoBase
    {
        public List<AdminsForEventsViewDto> AdminsForEvents { get; set; } = null!;
    }
}
