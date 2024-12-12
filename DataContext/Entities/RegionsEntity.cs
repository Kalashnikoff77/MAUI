namespace DataContext.Entities
{
    public class RegionsEntity : EntityBase
    {
        public string Name { get; set; } = null!;

        public int Number { get; set; }

        public int CountryId { get; set; }

        public short Order { get; set; }
    }
}
