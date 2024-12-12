namespace DataContext.Entities.Views
{
    public class SchedulesDatesViewEntity : EntityBase
    {
        public int EventId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
