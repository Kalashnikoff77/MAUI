using System.ComponentModel.DataAnnotations;

namespace DataContext.Entities.Views
{
    public class AccountsViewEntity : AccountsEntity
    {
        [Required]
        public string Users { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        public string? Avatar { get; set; }

        public string? Photos { get; set; }

        public string? Hobbies { get; set; }

        public string? Relations { get; set; }

        public string? Schedules { get; set; }

        [Required]
        public string? LastVisit { get; set; }
    }
}
