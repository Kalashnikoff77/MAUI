using Data.Dto;

namespace Data.Models.SignalR
{
    public class SignalGlobalRequest
    {
        public OnAvatarChanged? OnAvatarChanged { get; set; }
        public OnScheduleChanged? OnScheduleChanged { get; set; }
        public OnMessagesReload? OnMessagesReload { get; set; }
        public OnLastMessagesReload? OnLastMessagesReload { get; set; }
    }


    public class OnAvatarChanged
    {
        public PhotosForAccountsDto NewAvatar { get; set; } = null!;
    }

    public class OnScheduleChanged
    {
        public int? EventId { get; set; }
        public int ScheduleId { get; set; }
    }

    public class OnMessagesReload
    {
        public int? RecipientId { get; set; }
    }

    public class OnLastMessagesReload
    {
        public int? RecipientId { get; set; }
    }
}

