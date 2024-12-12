namespace DataContext.Entities.Views
{
    public class FeaturesForEventsViewEntity : EntityBase
    {
        public string Name { get; set; } = null!;

        public DateTime EndDate { get; set; }

        public int AdminId { get; set; }

        public int RegionId { get; set; }
    }
}
