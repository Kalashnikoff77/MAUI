namespace DataContext.Entities
{
    public class HobbiesEntity : EntityBase
    {
        public string Name { get; set; } = null!;
        
        public string? Description { get; set; }
    }
}
