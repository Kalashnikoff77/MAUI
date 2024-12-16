using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetPhotosForAccountsResponseDto : ResponseDtoBase
    {
        public List<PhotosForAccountsViewDto> Photos { get; set; } = null!;
    }
}
