using System.ComponentModel.DataAnnotations;

namespace DataContext.Entities.Views
{
    public class EventsViewEntity : EventsEntity
    {
        [Required]
        public string Admin { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        public string Avatar { get; set; } = null!;

        public string? Photos { get; set; }

        [Required]
        public string? Statistic { get; set; }
    }
}
