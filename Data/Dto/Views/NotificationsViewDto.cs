namespace Data.Dto.Views
{
    public class NotificationsViewDto : NotificationsDto
    {
        public AccountsViewDto? Sender { get; set; }
    }
}
