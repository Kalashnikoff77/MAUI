using Common.Dto.Responses;
using Common.Dto.Views;

namespace Common.Dto
{
    public class GetAdminsForEventsViewDto : ResponseDtoBase
    {
        public List<AdminsForEventsViewDto> AdminsForEvents { get; set; } = null!;
    }
}
