namespace Common.Dto.Views
{
    public class PhotosForAccountsViewDto : PhotosForAccountsDto
    {
        public AccountsViewDto? Account { get; set; }
    }
}
