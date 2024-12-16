namespace Data.Dto.Views
{
    public class DiscussionsForEventsViewDto : DiscussionsForEventsDto
    {
        public AccountsViewDto? Sender { get; set; }
        public AccountsViewDto? Recipient { get; set; }
    }
}
