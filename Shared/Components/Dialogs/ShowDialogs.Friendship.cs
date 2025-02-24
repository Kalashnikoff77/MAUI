using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Enums;
using Data.Extensions;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace Shared.Components.Dialogs
{
    public partial class ShowDialogs
    {
        /// <summary>
        /// Запрос на добавление в друзья
        /// </summary>
        public async Task SendFriendshipRequestDialogAsync(AccountsViewDto? sender, AccountsViewDto? recipient)
        {
            if (CurrentState.Account != null && sender != null && recipient != null)
            {
                var result = await ShowDialog("Отправить приглашение к дружбе?", "Да", Color.Success);

                if (result != null && result.Canceled == false)
                {
                    // 1. Добавим связь в таблицу RelationsForAccounts
                    var _repoUpdateRelation = _serviceProvider.GetService<IRepository<UpdateRelationRequestDto, UpdateRelationResponseDto>>()!;
                    var apiUpdateResponse = await _repoUpdateRelation.HttpPostAsync(new UpdateRelationRequestDto
                    {
                        RecipientId = recipient.Id,
                        EnumRelation = EnumRelations.Friend,
                        Token = CurrentState.Account.Token
                    });

                    // 2. Обновим CurrentState у обоих пользователей
                    await AccountsReload(recipient.Id);

                    // 3. Добавим информационное сообщение в Messages
                    var service = _serviceProvider.GetService<IRepository<AddMessageRequestDto, AddMessageResponseDto>>()!;
                    var apiResult = await service.HttpPostAsync(new AddMessageRequestDto
                    {
                        Type = EnumMessages.RequestForFriendshipSent,
                        RecipientId = recipient.Id,
                        Text = StaticData.NotificationTypes[EnumMessages.RequestForFriendshipSent].Text,
                        Token = CurrentState.Account.Token
                    });

                    // 4. Сообщим обоим пользователям о событии
                    var onMessagesUpdatedRequest = new SignalGlobalRequest
                    {
                        OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                        {
                            RecipientId = recipient.Id,
                            FriendshipRequest = true
                        }
                    };
                    await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                }
            }
        }

        /// <summary>
        /// Принятие запроса на добавления в друзья
        /// </summary>
        public async Task AcceptFriendshipRequestDialogAsync(AccountsViewDto? sender, AccountsViewDto? recipient)
        {
            if (CurrentState.Account != null && sender != null && recipient != null)
            {
                var result = await ShowDialog("Вы согласны принять дружбу?", "Да", Color.Success);

                if (result != null && result.Canceled == false)
                {
                    // 1. Добавим связь дружбы в БД
                    var _repoUpdateRelation = _serviceProvider.GetService<IRepository<UpdateRelationRequestDto, UpdateRelationResponseDto>>()!;
                    var apiUpdateResponse = await _repoUpdateRelation.HttpPostAsync(new UpdateRelationRequestDto
                    {
                        RecipientId = recipient.Id,
                        EnumRelation = EnumRelations.Friend,
                        Token = CurrentState.Account.Token
                    });

                    // 2. Обновим CurrentState у обоих пользователей
                    await AccountsReload(recipient.Id);

                    // 3. Получим сообщение о запросе дружбы из Messages
                    var _getMessage = _serviceProvider.GetService<IRepository<GetMessagesRequestDto, GetMessagesResponseDto>>()!;
                    var apiGetMessageResponse = await _getMessage.HttpPostAsync(new GetMessagesRequestDto
                    {
                        RecipientId = recipient.Id,
                        Type = EnumMessages.RequestForFriendshipSent,
                        Token = CurrentState.Account.Token
                    });

                    // 4. Удалим сообщение о запросе дружбы из Messages
                    if (apiGetMessageResponse.Response.Message != null)
                    {
                        var _deleteMessage = _serviceProvider.GetService<IRepository<DeleteMessagesRequestDto, ResponseDtoBase>>()!;
                        var apiDeleteResponse = await _deleteMessage.HttpPostAsync(new DeleteMessagesRequestDto
                        {
                            RecipientId = apiGetMessageResponse.Response.Message.SenderId,
                            MessageId = apiGetMessageResponse.Response.Message.Id,
                            Token = CurrentState.Account.Token
                        });
                    }

                    // 5. Добавим информационное сообщение в Messages
                    var service = _serviceProvider.GetService<IRepository<AddMessageRequestDto, AddMessageResponseDto>>()!;
                    var apiResult = await service.HttpPostAsync(new AddMessageRequestDto
                    {
                        Type = EnumMessages.RequestForFriendshipAccepted,
                        RecipientId = recipient.Id,
                        Text = StaticData.NotificationTypes[EnumMessages.RequestForFriendshipAccepted].Text,
                        Token = CurrentState.Account.Token
                    });

                    // 6. Сообщим обоим пользователям о событии
                    var onMessagesUpdatedRequest = new SignalGlobalRequest
                    {
                        OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                        {
                            AcceptFriendshipRequest = true,
                            RecipientId = recipient.Id
                        }
                    };
                    await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                }
            }
        }

        /// <summary>
        /// Отмена запроса инициатором на добавление в друзья
        /// </summary>
        public async Task AbortFriendshipRequestDialogAsync(AccountsViewDto? sender, AccountsViewDto? recipient)
        {
            if (CurrentState.Account != null && sender != null && recipient != null)
            {
                var result = await ShowDialog("Отменить запрос на дружбу?", "Да", Color.Error);

                if (result != null && result.Canceled == false)
                {
                    // 1. Удалим данные о запросе дружбы в RelationsForAccounts
                    var _deleteRelation = _serviceProvider.GetService<IRepository<DeleteRelationRequestDto, ResponseDtoBase>>()!;
                    var apiRelationResponse = await _deleteRelation.HttpPostAsync(new DeleteRelationRequestDto
                    {
                        RecipientId = recipient.Id,
                        EnumRelation = EnumRelations.Friend,
                        Token = CurrentState.Account.Token
                    });

                    // 2. Обновим CurrentState у обоих пользователей
                    await AccountsReload(recipient.Id);

                    // 3. Получим сообщение о запросе дружбы из Messages
                    var _getMessage = _serviceProvider.GetService<IRepository<GetMessagesRequestDto, GetMessagesResponseDto>>()!;
                    var apiGetMessageResponse = await _getMessage.HttpPostAsync(new GetMessagesRequestDto
                    {
                        RecipientId = recipient.Id,
                        Type = EnumMessages.RequestForFriendshipSent,
                        Token = CurrentState.Account.Token
                    });

                    // 4. Удалим сообщение о запросе дружбы из Messages
                    if (apiGetMessageResponse.Response.Message != null)
                    {
                        var _deleteMessage = _serviceProvider.GetService<IRepository<DeleteMessagesRequestDto, ResponseDtoBase>>()!;
                        var apiDeleteResponse = await _deleteMessage.HttpPostAsync(new DeleteMessagesRequestDto
                        {
                            RecipientId = recipient.Id,
                            MessageId = apiGetMessageResponse.Response.Message.Id,
                            Token = CurrentState.Account.Token
                        });
                    }

                    // 5. Сообщим обоим пользователям о событии
                    var onMessagesUpdatedRequest = new SignalGlobalRequest
                    {
                        OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                        {
                            AbortFriendshipRequest = true,
                            RecipientId = recipient.Id
                        }
                    };
                    await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                }
            }
        }

        /// <summary>
        /// Отказ принятия дружбы
        /// </summary>
        public async Task DeclineFriendshipRequestDialogAsync(AccountsViewDto? sender, AccountsViewDto? recipient)
        {
            if (CurrentState.Account != null && sender != null && recipient != null)
            {
                var result = await ShowDialog("Отклонить предложение дружбы?", "Да", Color.Error);

                if (result != null && result.Canceled == false)
                {
                    // 1. Удалим данные о запросе дружбы в RelationsForAccounts
                    var _deleteRelation = _serviceProvider.GetService<IRepository<DeleteRelationRequestDto, ResponseDtoBase>>()!;
                    var apiRelationResponse = await _deleteRelation.HttpPostAsync(new DeleteRelationRequestDto
                    {
                        RecipientId = recipient.Id,
                        EnumRelation = EnumRelations.Friend,
                        Token = CurrentState.Account.Token
                    });

                    // 2. Обновим CurrentState у обоих пользователей
                    await AccountsReload(recipient.Id);

                    // 3. Получим сообщение о запросе дружбы из Messages
                    var _getMessage = _serviceProvider.GetService<IRepository<GetMessagesRequestDto, GetMessagesResponseDto>>()!;
                    var apiGetMessageResponse = await _getMessage.HttpPostAsync(new GetMessagesRequestDto
                    {
                        RecipientId = recipient.Id,
                        Type = EnumMessages.RequestForFriendshipSent,
                        Token = CurrentState.Account.Token
                    });

                    // 4. Удалим сообщение о запросе дружбы из Messages
                    if (apiGetMessageResponse.Response.Message != null)
                    {
                        var _deleteMessage = _serviceProvider.GetService<IRepository<DeleteMessagesRequestDto, ResponseDtoBase>>()!;
                        var apiDeleteResponse = await _deleteMessage.HttpPostAsync(new DeleteMessagesRequestDto
                        {
                            RecipientId = apiGetMessageResponse.Response.Message.SenderId,
                            MessageId = apiGetMessageResponse.Response.Message.Id,
                            Token = CurrentState.Account.Token
                        });
                    }

                    // 5. Добавим информационное сообщение в Messages
                    var service = _serviceProvider.GetService<IRepository<AddMessageRequestDto, AddMessageResponseDto>>()!;
                    var apiResult = await service.HttpPostAsync(new AddMessageRequestDto
                    {
                        Type = EnumMessages.RequestForFriendshipDeclined,
                        RecipientId = recipient.Id,
                        Text = StaticData.NotificationTypes[EnumMessages.RequestForFriendshipDeclined].Text,
                        Token = CurrentState.Account.Token
                    });

                    // 6. Сообщим обоим пользователям о событии
                    var onMessagesUpdatedRequest = new SignalGlobalRequest
                    {
                        OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                        {
                            DeclineFriendshipRequest = true,
                            RecipientId = recipient.Id
                        }
                    };
                    await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                }
            }
        }

        /// <summary>
        /// Прекращение дружбы
        /// </summary>
        public async Task CancelFriendshipDialogAsync(AccountsViewDto? sender, AccountsViewDto? recipient)
        {
            if (CurrentState.Account != null && sender != null && recipient != null)
            {
                var hasFriendshipRelation = CurrentState.Account.Relations?.GetRelationInfo(EnumRelations.Friend, sender, recipient);

                var result = await ShowDialog("Вы хотите прекратить дружбу?", "Да", Color.Error);

                if (result != null && result.Canceled == false)
                {
                    // 1. Удалим данные о дружбе в RelationsForAccounts
                    var _repoUpdateRelation = _serviceProvider.GetService<IRepository<UpdateRelationRequestDto, UpdateRelationResponseDto>>()!;
                    var apiUpdateResponse = await _repoUpdateRelation.HttpPostAsync(new UpdateRelationRequestDto
                    {
                        RecipientId = recipient.Id,
                        EnumRelation = EnumRelations.Friend,
                        Token = CurrentState.Account.Token
                    });

                    // 2. Обновим состояния у обоих пользователей
                    await AccountsReload(recipient.Id);

                    // 3. Добавим информационное сообщение в Messages
                    var service = _serviceProvider.GetService<IRepository<AddMessageRequestDto, AddMessageResponseDto>>()!;
                    var apiResult = await service.HttpPostAsync(new AddMessageRequestDto
                    {
                        Type = EnumMessages.FriendshipCancelled,
                        RecipientId = recipient.Id,
                        Text = StaticData.NotificationTypes[EnumMessages.FriendshipCancelled].Text,
                        Token = CurrentState.Account.Token
                    });

                    // 4. Сообщим обоим пользователям о событии
                    var onMessagesUpdatedRequest = new SignalGlobalRequest
                    {
                        OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                        {
                            CancelFriendship = true,
                            RecipientId = recipient.Id
                        }
                    };
                    await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                }
            }
        }
    }
}
