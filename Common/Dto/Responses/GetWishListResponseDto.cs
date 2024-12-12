using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetWishListResponseDto : ResponseDtoBase
    {
        public List<WishListViewDto> WishList { get; set; } = null!;
    }
}
