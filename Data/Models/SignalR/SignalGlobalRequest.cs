using Data.Dto;

namespace Data.Models.SignalR
{
    public class SignalGlobalRequest
    {
        public OnAvatarChanged? OnAvatarChanged { get; set; }
        public OnScheduleChanged? OnScheduleChanged { get; set; }
        public OnGetNewMessages? OnGetNewMessages { get; set; }
        public OnMarkMessagesAsRead? OnMarkMessagesAsRead { get; set; }
        public OnUpdateLastMessages? OnUpdateLastMessages { get; set; }
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

    public class OnGetNewMessages
    {
        public int? RecipientId { get; set; }
    }

    public class OnMarkMessagesAsRead
    {
        public int RecipientId { get; set; }

        public IEnumerable<MessagesDto> Messages { get; set; } = null!;
    }

    public class OnUpdateLastMessages
    {
        public int RecipientId { get; set; }
    }
}

