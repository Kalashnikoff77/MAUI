using System.Text.Json;

namespace Data.Dto.Requests
{
    public class GetFeaturesRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/GetFeatures";
    }
}
