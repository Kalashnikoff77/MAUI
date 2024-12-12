using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetCountriesResponseDto : ResponseDtoBase
    {
        public List<CountriesViewDto> Countries { get; set; } = null!;
    }
}
