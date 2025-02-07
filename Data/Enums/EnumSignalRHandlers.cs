namespace Data.Enums
{
    public enum EnumSignalRHandlers
    {
        /// <summary>
        /// Пользователь залогинился
        /// </summary>
        UpdateOnlineAccountsClient,

        /// <summary>
        /// При добавлении обсуждения в мероприятие
        /// </summary>
        OnScheduleChangedClient,

        /// <summary>
        /// Пользователь изменил свой аватар
        /// </summary>
        OnAvatarChangedClient,

        ///// <summary>
        ///// Получаем новые сообщения для двух пользователей (диалоговое окно страницы /messages)
        ///// </summary>
        //OnGetNewMessagesClient,

        ///// <summary>
        ///// Обновляем сообщения в диалоговом окне страницы /messages (причина: сообщение прочитано)
        ///// </summary>
        //OnMarkMessagesAsReadClient,

        ///// <summary>
        ///// Обновление сообщения в диалоговом окне страницы /messages (причина: изменено или удалено)
        ///// </summary>
        //OnUpdateMessageClient,

        /// <summary>
        /// Изменения в таблице Messages
        /// </summary>
        OnMessagesUpdatedClient,

        ///// <summary>
        ///// Вызывается, когда меняется кол-во непрочитанных сообщений
        ///// </summary>
        //OnUpdateMessagesClient,

        /// <summary>
        /// Вызывается, когда изменяется связь с пользователями (блокировка, дружба и т.п.)
        /// </summary>
        OnUpdateAccountRelation,

        // Требуется проверить, используются ли эти перечисления
        //NewMessageAddedServer,
        //NewMessageAddedClient,

        //NewEventDiscussionAddedServer,
        //NewEventDiscussionAddedClient,

        //UpdateMessagesCountServer,
        //UpdateMessagesCountClient,

        //UpdateOnlineAccountsClient,

        //AvatarChangedServer,
        //AvatarChangedClient,

        //UpdateRelationsServer,
        //GetRelationsClient,
        //GetRelationsServer
    }
}
