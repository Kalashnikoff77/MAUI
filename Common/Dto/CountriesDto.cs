namespace Common.Dto
{
    public class CountriesDto : DtoBase
    {
        public string Name { get; set; } = null!;

        public RegionsDto Region { get; set; } = new RegionsDto();
    }
}
