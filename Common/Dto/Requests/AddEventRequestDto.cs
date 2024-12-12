using Common.Dto.Views;

namespace Common.Dto.Requests
{
    public class AddEventRequestDto : EventRequestDtoBase
    {
        public override string Uri => "/Events/Add";
    }
}
