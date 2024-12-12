namespace Common.Dto
{
    public class FeaturesDto : DtoBase
    {
        public string Name { get; set; } = null!;

        /// <summary>
        /// Для корректного вывода названия региона в выпадающем меню в Events.razor.cs
        /// </summary>
        public override string ToString() => Name;
    }
}
