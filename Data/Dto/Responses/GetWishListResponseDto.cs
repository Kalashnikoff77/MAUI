using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetWishListResponseDto : ResponseDtoBase
    {
        public List<WishListViewDto> WishList { get; set; } = null!;
    }
}
