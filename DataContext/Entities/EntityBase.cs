using System.ComponentModel.DataAnnotations;

namespace DataContext.Entities
{
    public abstract class EntityBase
    {
        [Required]
        public int Id { get; set; }
    }
}
