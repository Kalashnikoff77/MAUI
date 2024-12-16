using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetCountriesResponseDto : ResponseDtoBase
    {
        public List<CountriesViewDto> Countries { get; set; } = null!;
    }
}
