using System.Text.Json;

namespace Common.Dto.Requests
{
    public class GetRegionsForEventsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Countries/GetRegionsForEvents";
    }
}
