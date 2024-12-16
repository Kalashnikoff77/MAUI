using System.Text.Json;

namespace Data.Dto.Requests
{
    public class GetRegionsForEventsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Countries/GetRegionsForEvents";
    }
}
