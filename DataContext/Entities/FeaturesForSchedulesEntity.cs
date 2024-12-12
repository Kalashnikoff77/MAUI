namespace DataContext.Entities
{
    public class FeaturesForSchedulesEntity : EntityBase
    {
        public int ScheduleId { get; set; }
        public int FeatureId { get; set; }
    }
}
