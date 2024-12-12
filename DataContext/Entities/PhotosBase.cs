using System.ComponentModel.DataAnnotations;

namespace DataContext.Entities
{
    public class PhotosBase : EntityBase
    {
        [Required]
        public Guid Guid { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public int RelatedId { get; set; }

        [Required]
        public string? Comment { get; set; }

        [Required]
        public bool IsAvatar { get; set; }

        public bool IsDeleted { get; set; }

    }
}
