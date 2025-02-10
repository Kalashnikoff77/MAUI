using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnMessagesUpdatedResponse : SignalRModelBase<OnMessagesUpdatedResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnMessagesUpdatedClient;

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
        /// Удаление всей переписки в диалогах двух пользователей в MessagesDialog
        /// </summary>
        public bool DeleteMessages { get; set; }

        /// <summary>
        /// Пометить сообщения как прочитанные
        /// </summary>
        public bool MarkMessagesAsRead { get; set; }
    }
}
