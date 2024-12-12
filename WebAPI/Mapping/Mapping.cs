using AutoMapper;
using Common.Dto;
using Common.Dto.Functions;
using Common.Dto.Sp;
using Common.Dto.Views;
using DataContext.Entities;
using DataContext.Entities.Sp;
using DataContext.Entities.Views;
using WebAPI.Mapping.Converters;

namespace WebAPI.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AccountsViewEntity, AccountsViewDto>()
                .ForMember(to => to.Users, from => from.ConvertUsing<JsonToClassConverter<List<UsersDto>>, string?>(from => from.Users))
                .ForMember(to => to.Country, from => from.ConvertUsing<JsonToClassConverter<CountriesDto>, string?>(from => from.Country))
                .ForMember(to => to.Avatar, from => from.ConvertUsing<JsonToClassConverter<PhotosForAccountsDto>, string?>(from => from.Avatar))
                .ForMember(to => to.Photos, from => from.ConvertUsing<JsonToClassConverter<List<PhotosForAccountsDto>>, string?>(from => from.Photos))
                .ForMember(to => to.Hobbies, from => from.ConvertUsing<JsonToClassConverter<List<HobbiesDto>>, string?>(from => from.Hobbies))
                .ForMember(to => to.Relations, from => from.ConvertUsing<JsonToClassConverter<List<RelationsForAccountsDto>>, string?>(from => from.Relations))
                .ForMember(to => to.Schedules, from => from.ConvertUsing<JsonToClassConverter<List<SchedulesForAccountsDto>>, string?>(from => from.Schedules));

            CreateMap<CountriesViewEntity, CountriesViewDto>()
                .ForMember(to => to.Regions, from => from.ConvertUsing<JsonToClassConverter<List<RegionsDto>>, string?>(from => from.Regions));

            CreateMap<RegionsForEventsViewEntity, RegionsForEventsViewDto>();
            CreateMap<AdminsForEventsViewEntity, AdminsForEventsViewDto>();
            CreateMap<FeaturesForEventsViewEntity, FeaturesForEventsViewDto>();

            CreateMap<EventsViewEntity, EventsViewDto>()
                .ForMember(to => to.Admin, from => from.ConvertUsing<JsonToClassConverter<AccountsViewDto>, string?>(from => from.Admin))
                .ForMember(to => to.Country, from => from.ConvertUsing<JsonToClassConverter<CountriesDto>, string?>(from => from.Country))
                .ForMember(to => to.Avatar, from => from.ConvertUsing<JsonToClassConverter<PhotosForEventsDto>, string?>(from => from.Avatar))
                .ForMember(to => to.Photos, from => from.ConvertUsing<JsonToClassConverter<List<PhotosForEventsDto>>, string?>(from => from.Photos))
                .ForMember(to => to.Statistic, from => from.ConvertUsing<JsonToClassConverter<GetEventStatisticFunctionDto>, string?>(from => from.Statistic));

            CreateMap<SchedulesForEventsViewEntity, SchedulesForEventsViewDto>()
                .ForMember(to => to.Event, from => from.ConvertUsing<JsonToClassConverter<EventsViewDto>, string?>(from => from.Event))
                .ForMember(to => to.Features, from => from.ConvertUsing<JsonToClassConverter<List<FeaturesDto>>, string?>(from => from.Features))
                .ForMember(to => to.Statistic, from => from.ConvertUsing<JsonToClassConverter<GetScheduleStatisticFunctionDto>, string?>(from => from.Statistic));

            CreateMap<SchedulesForAccountsViewEntity, SchedulesForAccountsViewDto>()
                .ForMember(to => to.Account, from => from.ConvertUsing<JsonToClassConverter<AccountsViewDto>, string?>(from => from.Account));

            CreateMap<DiscussionsForEventsViewEntity, DiscussionsForEventsViewDto>()
                .ForMember(to => to.Sender, from => from.ConvertUsing<JsonToClassConverter<AccountsViewDto>, string?>(from => from.Sender))
                .ForMember(to => to.Recipient, from => from.ConvertUsing<JsonToClassConverter<AccountsViewDto>, string?>(from => from.Recipient));

            CreateMap<WishListViewEntity, WishListViewDto>()
                .ForMember(to => to.Users, from => from.ConvertUsing<JsonToClassConverter<List<UsersDto>>, string?>(from => from.Users))
                .ForMember(to => to.Country, from => from.ConvertUsing<JsonToClassConverter<CountriesDto>, string?>(from => from.Country))
                .ForMember(to => to.Avatar, from => from.ConvertUsing<JsonToClassConverter<PhotosForAccountsDto>, string?>(from => from.Avatar))
                .ForMember(to => to.Relations, from => from.ConvertUsing<JsonToClassConverter<List<RelationsForAccountsDto>>, string?>(from => from.Relations));

            CreateMap<LastMessagesForAccountSpEntity, LastMessagesForAccountSpDto>()
                .ForMember(to => to.Sender, from => from.ConvertUsing<JsonToClassConverter<AccountsViewDto>, string?>(from => from.Sender))
                .ForMember(to => to.Recipient, from => from.ConvertUsing<JsonToClassConverter<AccountsViewDto>, string?>(from => from.Recipient));

            CreateMap<NotificationsViewEntity, NotificationsViewDto>()
                .ForMember(to => to.Sender, from => from.ConvertUsing<JsonToClassConverter<AccountsViewDto>, string?>(from => from.Sender));

            CreateMap<PhotosForAccountsViewEntity, PhotosForAccountsViewDto>()
                .ForMember(to => to.Account, from => from.ConvertUsing<JsonToClassConverter<AccountsViewDto>, string?>(from => from.Account));

            CreateMap<SchedulesDatesViewEntity, SchedulesDatesViewDto>();

            CreateMap<IdentitiesEntity, IdentitiesDto>();
            CreateMap<InformingsEntity, InformingsDto>();
            CreateMap<MessagesEntity, MessagesDto>();
            CreateMap<AccountsEntity, AccountsDto>();
            CreateMap<FeaturesEntity, FeaturesDto>();
            CreateMap<HobbiesEntity, HobbiesDto>();
        }
    }
}
