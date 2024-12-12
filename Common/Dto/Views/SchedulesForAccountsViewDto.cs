namespace Common.Dto.Views
{
    public class SchedulesForAccountsViewDto : SchedulesForAccountsDto
    {
        public AccountsViewDto? Account { get; set; } = null!;
    }
}
