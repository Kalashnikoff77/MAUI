namespace Common.Dto
{
    public class PhotosDtoBase : DtoBase
    {
        public Guid Guid { get; set; }

        public DateTime CreateDate { get; set; }

        public int RelatedId { get; set; }

        public string? Comment { get; set; }

        public bool IsAvatar { get; set; }

        public bool IsDeleted { get; set; }
    }
}
