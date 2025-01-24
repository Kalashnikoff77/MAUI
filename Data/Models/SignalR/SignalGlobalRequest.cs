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
        /// <summary>
        /// Второй участник в диалоге (первый берётся из [Authorize])
        /// </summary>
        public int? RecipientId { get; set; }
    }

    public class OnMarkMessagesAsRead
    {
        /// <summary>
        /// Пользователь, у которого отмечаются сообщения как прочитанные
        /// </summary>
        public int RecipientId { get; set; }

        /// <summary>
        /// Список Id сообщений для отметки (null - помечать все)
        /// </summary>
        public IEnumerable<int>? MessagesIds { get; set; } = null!;
    }

    public class OnUpdateLastMessages
    {
        /// <summary>
        /// Второй участник в диалоге (первый берётся из [Authorize])
        /// </summary>
        public int RecipientId { get; set; }
    }
}

