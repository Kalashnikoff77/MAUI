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

        /// <summary>
        /// Получаем новые сообщения для двух пользователей (диалоговое окно страницы /messages)
        /// </summary>
        OnGetNewMessagesClient,

        /// <summary>
        /// Обновляем сообщения в диалоговом окне страницы /messages (причины: сообщение отредактировано или прочитано)
        /// </summary>
        OnMarkMessagesAsReadClient,

        /// <summary>
        /// Вызывается, когда меняется кол-во непрочитанных сообщений
        /// </summary>
        OnUpdateMessagesCountClient,

        /// <summary>
        /// Вызывается, когда меняется кол-во непрочитанных уведомлений
        /// </summary>
        OnUpdateNotificationsCountClient,

        // Требуется проверить, используются ли эти перечисления
        //NewMessageAddedServer,
        //NewMessageAddedClient,

        //NewEventDiscussionAddedServer,
        //NewEventDiscussionAddedClient,

        //NewNotificationAddedServer,
        //NewNotificationAddedClient,

        //UpdateMessagesCountServer,
        //UpdateMessagesCountClient,

        //UpdateNotificationsCountServer,
        //UpdateNotificationsCountClient,

        //UpdateOnlineAccountsClient,

        //AvatarChangedServer,
        //AvatarChangedClient,

        //UpdateRelationsServer,
        //GetRelationsClient,
        //GetRelationsServer
    }
}
