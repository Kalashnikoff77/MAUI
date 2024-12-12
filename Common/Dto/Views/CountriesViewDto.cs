namespace Common.Dto.Views
{
    public class CountriesViewDto : CountriesDto
    {
        public List<RegionsDto>? Regions { get; set; }
    }
}
