using System.Text.Json;

namespace Data.Dto.Requests
{
    public class GetCountriesRequestDto : RequestDtoBase
    {
        public override string Uri => "/Countries/Get";

        public int? CountryId { get; set; }
    }
}
