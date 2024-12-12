namespace Common.Dto.Views
{
    public class FeaturesForEventsViewDto : FeaturesDto
    {
        public DateTime EndDate { get; set; }

        public int AdminId { get; set; }

        public int RegionId { get; set; }

        public int NumberOfEvents { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Для корректного вывода названия услуги в выпадающем меню в Events.razor.cs
        /// </summary>
        public override string ToString() => Name;
    }
}
