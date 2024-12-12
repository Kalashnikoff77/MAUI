using System.ComponentModel.DataAnnotations;

namespace DataContext.Entities
{
    public class AccountsEntity : EntityBase
    {
        [Required]
        public Guid Guid { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public bool IsConfirmed { get; set; }

        public int RegionId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
