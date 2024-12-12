using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetPhotosForAccountsResponseDto : ResponseDtoBase
    {
        public List<PhotosForAccountsViewDto> Photos { get; set; } = null!;
    }
}
