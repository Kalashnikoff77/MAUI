namespace DataContext.Entities.Views
{
    public class AdminsForEventsViewEntity : EntityBase
    {
        public string Name { get; set; } = null!;

        public DateTime EndDate { get; set; }

        public int RegionId { get; set; }

        public int FeatureId { get; set; }
    }
}
