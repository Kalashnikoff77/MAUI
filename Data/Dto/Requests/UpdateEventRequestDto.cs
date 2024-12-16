using Data.Dto.Views;

namespace Data.Dto.Requests
{
    public class UpdateEventRequestDto : EventRequestDtoBase
    {
        public override string Uri => "/Events/Update";
    }
}
