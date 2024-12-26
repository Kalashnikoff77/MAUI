using Data.Dto;

namespace Data.Models.SignalR
{
    public class SignalGlobalRequest
    {
        public OnAvatarChanged? OnAvatarChanged { get; set; }
        public OnScheduleChanged? OnScheduleChanged { get; set; }
        public OnMessageAdded? OnMessageAdded { get; set; }
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

    public class OnMessageAdded
    {
        public MessagesDto Message { get; set; } = null;
    }

}

