namespace Data.Enums
{
    /// <summary>
    /// Типы сообщений в таблице Messages
    /// </summary>
    public enum EnumMessages : short
    {
        /// <summary>
        /// Обычное сообщение
        /// </summary>
        Message,
        
        /// <summary>
        /// Отправлен запрос на добавление в друзья
        /// </summary>
        RequestForFrendshipSent,
        
        /// <summary>
        /// Запрос на добавление в друзья принят
        /// </summary>
        RequestForFrendshipAccepted,

        /// <summary>
        /// Запрос на добавление в друзья отклонён
        /// </summary>
        RequestForFrendshipDeclined,

        /// <summary>
        /// Все сообщения удалены
        /// </summary>
        AllMessagesDeleted,

        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        AccountBlocked,

        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        AccountUnblocked
    }
}
