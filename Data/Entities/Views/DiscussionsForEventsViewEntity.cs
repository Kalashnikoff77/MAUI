using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Views
{
    public class DiscussionsForEventsViewEntity : DiscussionsForEventsEntity
    {
        [Required]
        public string? Sender { get; set; }
        [Required]
        public string? Recipient { get; set; }
    }
}
