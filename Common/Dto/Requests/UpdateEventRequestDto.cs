using Common.Dto.Views;

namespace Common.Dto.Requests
{
    public class UpdateEventRequestDto : EventRequestDtoBase
    {
        public override string Uri => "/Events/Update";
    }
}
