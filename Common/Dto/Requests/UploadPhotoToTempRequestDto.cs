namespace Common.Dto.Requests
{
    public class UploadPhotoToTempRequestDto : RequestDtoBase
    {
        public override string Uri => "/Photos/UploadPhotoToTemp";

        public int? AccountId { get; set; }

        public int? EventId { get; set; }

        /// <summary>
        /// Само фото
        /// </summary>
        public byte[]? File { get; set; }
    }
}
