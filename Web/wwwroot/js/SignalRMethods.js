// Пометить аватары онлайн пользователей
function UpdateOnlineAccountsClient(connectedAccounts) {
    $('img[data-avatar]').removeClass('img-online');

    connectedAccounts.forEach(id => {
        console.log('Account Online: ' + id);
        $('img[data-avatar=' + id + ']').addClass('img-online');
    })
}

// Пользователь сменил аватар
function OnAvatarChangedClient(onAvatarChangedResponse) {
    // Если isAvatar = true, значит отменяется текущий аватар (пользователь остаётся без аватара)
    if (onAvatarChangedResponse.newAvatar.isAvatar)
        var newPath = '/images/AccountsPhotos/no-avatar';
    else
        var newPath = '/images/AccountsPhotos/' + onAvatarChangedResponse.newAvatar.accountId + '/' + onAvatarChangedResponse.newAvatar.guid;

    $('img[data-avatar=' + onAvatarChangedResponse.newAvatar.accountId + ']')
        .each(function () {
            $(this).fadeOut(120, function () {
                $(this).attr('src', newPath + '/s64x64.jpg').fadeIn(120);
                $(this).attr('src', newPath + '/s150x150.jpg').fadeIn(120);
                $(this).attr('src', newPath + '/s250x250.jpg').fadeIn(120);
            })
        });
}



// Обновление кол-ва подписавшихся или зарегистрированных на мероприятие
function UpdateEventRegisterClient(updateEventRegisterModel) {
    updateEventRegisterModel.events.forEach(evt => {
        ChangeNumberFadeInOut('sw-event-registered-' + evt.id, evt.numOfRegisters, true);
    })
}




// Обновление кнопок связей с пользователем (дружба, блокировка и т.п.)
function GetRelationsClient(getRelationsModel) {
    var senderId = getRelationsModel.senderId;
    var recipientId = getRelationsModel.recipientId;

    // Если массив = null, то сформируем пустой массив для корректной работы linq в JS.
    if (getRelationsModel.relations == null)
        getRelationsModel.relations = [];

    $('[class^="sw-relation-"][class$="-' + recipientId + '"]').attr('hidden', !false);
    $('[class^="sw-relation-"][class$="-' + senderId + '"]').attr('hidden', !false);

    // БЛОКИРОВКА
    // Блокировка содержит только одну запись для двух пользователей
    var relationBlocked = (getRelationsModel.relations.where(x => x.type == EnumRelations.Blocked))[0];
    if (relationBlocked) {
        $('.sw-relation-tounblock-' + relationBlocked.recipientId + ', .sw-relation-blocked-' + relationBlocked.senderId).attr('hidden', !true);
        return;
    }
    $('[class^="sw-relation-toblock-"').attr('hidden', !true);

    // ПОДПИСКА
    // В relations может быть две записи по подписке (для каждого пользователя), поэтому делаем две проверки
    var isRelationSubscriber = getRelationsModel.relations.any(x => x.senderId == senderId && x.type == EnumRelations.Subscriber);
    $('.sw-relation-tounsubscribe-' + recipientId).attr('hidden', !isRelationSubscriber);
    $('.sw-relation-tosubscribe-' + recipientId).attr('hidden', isRelationSubscriber);

    isRelationSubscriber = getRelationsModel.relations.any(x => x.senderId == recipientId && x.type == EnumRelations.Subscriber);
    $('.sw-relation-tounsubscribe-' + senderId).attr('hidden', !isRelationSubscriber);
    $('.sw-relation-tosubscribe-' + senderId).attr('hidden', isRelationSubscriber);

    // ДРУЖБА
    // Дружбы нет
    var isRelationFriendship = getRelationsModel.relations.any(x => x.type == EnumRelations.Friendship);
    if (!isRelationFriendship) {
        $('.sw-relation-tomakefriendship-' + recipientId + ', .sw-relation-tomakefriendship-' + senderId).attr('hidden', !true);
        return;
    }

    // Дружба подтверждена
    var isRelationFriendshipConfirmed = getRelationsModel.relations.any(x => x.type == EnumRelations.Friendship && x.isConfirmed);
    if (isRelationFriendshipConfirmed) {
        $('.sw-relation-friendship-cancel-' + recipientId + ', .sw-relation-friendship-cancel-' + senderId).attr('hidden', !true);
        return;
    }

    // Дружба не подтверждена
    var relationFriendshipNotConfirmed = getRelationsModel.relations.where(x => x.type == EnumRelations.Friendship && x.isConfirmed == false)[0];
    if (relationFriendshipNotConfirmed) {
        $('.sw-relation-friendship-needs-confirm-' + relationFriendshipNotConfirmed.recipientId + ', .sw-relation-friendship-request-sent-' + relationFriendshipNotConfirmed.senderId).attr('hidden', !true);
    }
}
