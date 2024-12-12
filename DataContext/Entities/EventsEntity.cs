using System.ComponentModel.DataAnnotations;

namespace DataContext.Entities
{
    public class EventsEntity : EntityBase
    {
        [Required]
        public Guid Guid { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public int AdminId { get; set; }

        public int RegionId { get; set; }

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public short? MaxMen { get; set; }
        [Required]
        public short? MaxWomen { get; set; }
        [Required]
        public short? MaxPairs { get; set; }

        public bool IsDeleted { get; set; }
    }
}
