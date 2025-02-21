using Data.Dto;

namespace Data.Models.SignalR
{
    public class SignalGlobalRequest
    {
        public OnReloadAccountRequest? OnReloadAccountRequest { get; set; }
        public OnAvatarChangedRequest? OnAvatarChangedRequest { get; set; }
        public OnScheduleUpdatedRequest? OnScheduleUpdatedRequest { get; set; }
        public OnMessagesUpdatedRequest? OnMessagesUpdatedRequest { get; set; }
    }

    /// <summary>
    /// Вызывается, когда нужно перезагрузить состояние пользователя
    /// </summary>
    public class OnReloadAccountRequest
    {
        /// <summary>
        /// Дополнительно перезагрузить второго пользователя (первый берётся из [Authorize])
        /// </summary>
        public int? AdditionalAccountId { get; set; }
    }

    /// <summary>
    /// Пользователь изменил аватар
    /// </summary>
    public class OnAvatarChangedRequest
    {
        public PhotosForAccountsDto NewAvatar { get; set; } = null!;
    }

    /// <summary>
    /// Изменения в расписании
    /// </summary>
    public class OnScheduleUpdatedRequest
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
        public bool DeleteMessage { get; set; }

        /// <summary>
        /// Пометить сообщения как прочитанные
        /// </summary>
        public bool MarkMessagesAsRead { get; set; }

        /// <summary>
        /// Пометить сообщения как прочитанные
        /// </summary>
        public bool MarkAllMessagesAsRead { get; set; }

        /// <summary>
        /// Удалить одно или несколько сообщений
        /// </summary>
        public bool DeleteMessages { get; set; }

        /// <summary>
        /// Подружиться
        /// </summary>
        public bool FriendshipRequest { get; set; }

        /// <summary>
        /// Принятие запроса на добавления в друзья
        /// </summary>
        public bool AcceptFriendshipRequest { get; set; }

        /// <summary>
        /// Отмена запроса инициатором на добавление в друзья
        /// </summary>
        public bool AbortFriendshipRequest { get; set; }

        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        public bool BlockAccount { get; set; }

        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        public bool UnblockAccount { get; set; }
    }
}

