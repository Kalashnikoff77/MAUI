using System.Text.Json;

namespace Data.Dto.Requests
{
    public class GetFeaturesForEventsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/GetFeaturesForEvents";
    }
}
