namespace DataContext.Entities.Views
{
    public class RegionsForEventsViewEntity : EntityBase
    {
        public string Name { get; set; } = null!;

        public int Order { get; set; }

        public DateTime EndDate { get; set; }

        public int AdminId { get; set; }

        public int FeatureId { get; set; }
    }
}
