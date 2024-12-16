using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public abstract class EntityBase
    {
        [Required]
        public int Id { get; set; }
    }
}
