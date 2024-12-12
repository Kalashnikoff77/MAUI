namespace Common.Dto.Views
{
    public class AdminsForEventsViewDto : DtoBase
    {
        public string Name { get; set; } = null!;

        public DateTime EndDate { get; set; }

        public int RegionId { get; set; }

        public int FeatureId { get; set; }

        public int NumberOfEvents { get; set; }

        /// <summary>
        /// Для корректного вывода названия региона в выпадающем меню в Events.razor.cs
        /// </summary>
        public override string ToString() => Name;
    }
}
