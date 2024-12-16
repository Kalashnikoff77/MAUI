namespace Data.Entities
{
    public class CountriesEntity : EntityBase
    {
        public short Order { get; set; }
        public string Name { get; set; } = null!;
    }
}
