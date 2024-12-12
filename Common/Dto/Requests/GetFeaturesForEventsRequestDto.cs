using System.Text.Json;

namespace Common.Dto.Requests
{
    public class GetFeaturesForEventsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/GetFeaturesForEvents";
    }
}
