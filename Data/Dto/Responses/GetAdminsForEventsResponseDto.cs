using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetAdminsForEventsResponseDto : ResponseDtoBase
    {
        public List<AdminsForEventsViewDto> AdminsForEvents { get; set; } = null!;
    }
}
