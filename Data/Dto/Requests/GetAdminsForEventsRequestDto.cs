using System.Text.Json;

namespace Data.Dto.Requests
{
    public class GetAdminsForEventsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/GetAdminsForEvents";
    }
}
