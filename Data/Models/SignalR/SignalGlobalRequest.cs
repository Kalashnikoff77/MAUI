using Data.Dto;

namespace Data.Models.SignalR
{
    public class SignalGlobalRequest
    {
        public OnReloadAccount? OnReloadAccount { get; set; }
        public OnAvatarChanged? OnAvatarChanged { get; set; }
        public OnScheduleChanged? OnScheduleChanged { get; set; }
        public OnMessagesUpdatedRequest? OnMessagesUpdatedRequest { get; set; }
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

    /// <summary>
    /// Пользователь изменил аватар
    /// </summary>
    public class OnAvatarChanged
    {
        public PhotosForAccountsDto NewAvatar { get; set; } = null!;
    }

    /// <summary>
    /// Изменения в расписании
    /// </summary>
    public class OnScheduleChanged
    {
        public int? EventId { get; set; }
        public int ScheduleId { get; set; }
    }

    /// <summary>
    /// Изменения в таблице Messages
    /// </summary>
    public class OnMessagesUpdatedRequest
    {
        public int? RecipientId { get; set; }

        public int? MessageId { get; set; }
        public IEnumerable<int>? MessagesIds { get; set; }

        /// <summary>
        /// Добавление сообщения в диалог двух пользователей в MessagesDialog
        /// </summary>
        public bool AppendNewMessages { get; set; }

        /// <summary>
        /// Обновление или удаление сообщения в диалогах двух пользователей в MessagesDialog
        /// </summary>
        public bool UpdateMessage { get; set; }

        /// <summary>
        /// Пометить сообщения как прочитанные
        /// </summary>
        public bool MarkMessagesAsRead { get; set; }
    }
}

