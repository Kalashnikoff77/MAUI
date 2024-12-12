using System.ComponentModel.DataAnnotations;

namespace DataContext.Entities
{
    public class SchedulesForEventsEntity : EntityBase
    {
        [Required]
        public int EventId { get; set; }

        public Guid Guid { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int? CostMan { get; set; }
        [Required]
        public int? CostWoman { get; set; }
        [Required]
        public int? CostPair { get; set; }

        public bool IsDeleted { get; set; }
    }
}
