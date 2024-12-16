using Data.Dto.Responses;
using Data.Dto.Views;

namespace Data.Dto
{
    public class GetAdminsForEventsViewDto : ResponseDtoBase
    {
        public List<AdminsForEventsViewDto> AdminsForEvents { get; set; } = null!;
    }
}
