namespace Common.Dto.Responses
{
    public class UploadPhotoToTempResponseDto : ResponseDtoBase
    {
        public PhotosForAccountsDto? NewAccountPhoto { get; set; }
        public PhotosForEventsDto? NewEventPhoto { get; set; }
    }
}
