using Data.Dto;

namespace Data.Models.SignalR
{
    public class SignalGlobalRequest
    {
        public OnAvatarChanged? OnAvatarChanged { get; set; }
        public OnScheduleChanged? OnScheduleChanged { get; set; }
        public OnGetNewMessages? OnGetNewMessages { get; set; }
        public OnMarkMessagesAsRead? OnMarkMessagesAsRead { get; set; }
        public OnUpdateMessage? OnUpdateMessage { get; set; }
        public OnUpdateMessagesCount? OnUpdateMessagesCount { get; set; }

        public OnReloadAccount? OnReloadAccount { get; set; }
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

    /// <summary>
    /// Вызывается, когда добавляется новое сообщение
    /// </summary>
    public class OnGetNewMessages
    {
        /// <summary>
        /// Второй участник в диалоге (первый берётся из [Authorize])
        /// </summary>
        public int? RecipientId { get; set; }
    }

    /// <summary>
    /// Вызывается, когда сообщения помечаютя как прочитанные
    /// </summary>
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

    /// <summary>
    /// Вызывается, когда меняется кол-во непрочитанных сообщений
    /// </summary>
    public class OnUpdateMessagesCount
    {
        /// <summary>
        /// Второй участник в диалоге (первый берётся из [Authorize])
        /// </summary>
        public int RecipientId { get; set; }
    }

    /// <summary>
    /// Вызывается, когда изменяется или удаляется одно сообщение
    /// </summary>
    public class OnUpdateMessage
    {
        /// <summary>
        /// Id сообщения
        /// </summary>
        public int MessageId { get; set; }
        
        /// <summary>
        /// Второй участник в диалоге (первый берётся из [Authorize])
        /// </summary>
        public int RecipientId { get; set; }
    }


    /// <summary>
    /// Вызывается, когда нужно перезагрузить состояние пользователя
    /// </summary>
    public class OnReloadAccount
    {
        /// <summary>
        /// Дополнительно перезагрузить второго пользователя (первый берётся из [Authorize])
        /// </summary>
        public int? AdditionalAccountId { get; set; }
    }
}

