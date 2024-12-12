namespace Common.Dto.Views
{
    public class AccountsViewDto : AccountsDto
    {
        public List<UsersDto>? Users { get; set; }

        public CountriesDto? Country { get; set; }

        public PhotosForAccountsDto? Avatar { get; set; }

        public List<PhotosForAccountsDto>? Photos { get; set; }

        public List<EventsViewDto>? Events { get; set; }

        public List<HobbiesDto>? Hobbies { get; set; }

        public List<RelationsForAccountsDto>? Relations { get; set; }

        public List<SchedulesForAccountsDto>? Schedules { get; set; }

        public InformingsDto? Informings { get; set; }
    }
}
