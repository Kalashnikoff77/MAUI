using System.Text.Json;

namespace Common.Dto.Requests
{
    public class GetAdminsForEventsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/GetAdminsForEvents";
    }
}
