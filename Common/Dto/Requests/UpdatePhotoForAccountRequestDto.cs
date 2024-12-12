namespace Common.Dto.Requests
{
    public class UpdatePhotoForAccountRequestDto : RequestDtoBase
    {
        public override string Uri => "/Photos/UpdatePhotoForAccount";

        public Guid Guid { get; set; }

        public bool IsAvatarChanging { get; set; }

        public bool IsCommentChanging { get; set; }

        public bool IsDeleting { get; set; }

        public string? Comment { get; set; }
    }
}
