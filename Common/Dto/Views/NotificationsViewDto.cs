namespace Common.Dto.Views
{
    public class NotificationsViewDto : NotificationsDto
    {
        public AccountsViewDto? Sender { get; set; }
    }
}
