using Data.Dto.Views;

namespace Data.Dto.Requests
{
    public class AddEventRequestDto : EventRequestDtoBase
    {
        public override string Uri => "/Events/Add";
    }
}
