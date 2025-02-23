using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Sp;
using Data.Dto.Views;
using Data.Enums;
using Data.Extensions;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Shared.Components.Pages.Account;
using Shared.Components.Pages.Events;
using Shared.Components.Pages.Messages;

namespace Shared.Components.Dialogs
{
    public class ShowDialogs
    {
        public CurrentState CurrentState { get; set; } = null!;

        IServiceProvider _serviceProvider { get; set; } = null!;
        IDialogService _dialogService { get; set; } = null!;

        public ShowDialogs(IDialogService dialogService, CurrentState CurrentState, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dialogService = dialogService;
            this.CurrentState = CurrentState;
        }

        /// <summary>
        /// Карточка аккаунта
        /// </summary>
        public async Task AccountInfoCardDialogAsync(AccountsViewDto account)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true, BackdropClick = true };

            var dialogParams = new DialogParameters<AccountInfoCardDialog>
            {
                { x => x.Account, account }
            };
            await _dialogService.ShowAsync<AccountInfoCardDialog>(account.Name, dialogParams, dialogOptions);
        }

        /// <summary>
        /// Карточка мероприятия
        /// </summary>
        public async Task ScheduleInfoCardDialogAsync(SchedulesForEventsViewDto schedule)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true, BackdropClick = true };

            var dialogParams = new DialogParameters<ScheduleInfoCardDialog>
            {
                { x => x.ScheduleId, schedule.Id }
            };
            await _dialogService.ShowAsync<ScheduleInfoCardDialog>(schedule.Event?.Name, dialogParams, dialogOptions);
        }

        /// <summary>
        /// Подтверждение регистрация на клубное мероприятие
        /// </summary>
        public async Task RegistrationForEventDialogAsync(SchedulesForEventsViewDto schedule, bool isRegistered)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true };

            var dialogParams = new DialogParameters<RegisterForScheduleDialog>
            {
                { x => x.ScheduleForEventView, schedule },
                { x => x.IsRegistered, isRegistered }
            };
            await _dialogService.ShowAsync<RegisterForScheduleDialog>($"Подтверждение регистрации", dialogParams, dialogOptions);
        }

        /// <summary>
        /// Окно общения двух пользователей
        /// </summary>
        public async Task MessagesDialogAsync(AccountsViewDto account)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true };

            var dialogParams = new DialogParameters<MessagesDialog>
            {
                { x => x.Recipient, account }
            };
            await _dialogService.ShowAsync<MessagesDialog>(null, dialogParams, dialogOptions);
        }


        /// <summary>
        /// Запрос на добавление в друзья
        /// </summary>
        public async Task SendFriendshipRequestDialogAsync(AccountsViewDto? sender, AccountsViewDto? recipient)
        {
            if (CurrentState.Account != null && sender != null && recipient != null)
            {
                var parameters = new DialogParameters<ConfirmDialog>
                {
                    { x => x.ContentText, "Отправить приглашение к дружбе?" },
                    { x => x.ButtonText, "Да" },
                    { x => x.Color, Color.Success }
                };
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
                var resultDialog = await _dialogService.ShowAsync<ConfirmDialog>($"Подтверждение", parameters, options);
                var result = await resultDialog.Result;

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
                    var accountReloadRequest = new SignalGlobalRequest
                    {
                        OnReloadAccountRequest = new OnReloadAccountRequest { AdditionalAccountId = recipient.Id }
                    };
                    await CurrentState.SignalRServerAsync(accountReloadRequest);

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
                var parameters = new DialogParameters<ConfirmDialog>
                {
                    { x => x.ContentText, "Вы согласны принять дружбу?" },
                    { x => x.ButtonText, "Да" },
                    { x => x.Color, Color.Success }
                };
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
                var resultDialog = await _dialogService.ShowAsync<ConfirmDialog>($"Подтверждение", parameters, options);
                var result = await resultDialog.Result;

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
                    var accountReloadRequest = new SignalGlobalRequest
                    {
                        OnReloadAccountRequest = new OnReloadAccountRequest { AdditionalAccountId = recipient.Id }
                    };
                    await CurrentState.SignalRServerAsync(accountReloadRequest);

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
                var parameters = new DialogParameters<ConfirmDialog>
                {
                    { x => x.ContentText, $"Отменить запрос на дружбу?" },
                    { x => x.ButtonText, "Да" },
                    { x => x.Color, Color.Error }
                };
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
                var resultDialog = await _dialogService.ShowAsync<ConfirmDialog>($"Подтверждение", parameters, options);
                var result = await resultDialog.Result;

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
                    var accountReloadRequest = new SignalGlobalRequest
                    {
                        OnReloadAccountRequest = new OnReloadAccountRequest { AdditionalAccountId = recipient.Id }
                    };
                    await CurrentState.SignalRServerAsync(accountReloadRequest);

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
                var parameters = new DialogParameters<ConfirmDialog>
                {
                    { x => x.ContentText, $"Отклонить предложение дружбы?" },
                    { x => x.ButtonText, "Да" },
                    { x => x.Color, Color.Error }
                };
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
                var resultDialog = await _dialogService.ShowAsync<ConfirmDialog>($"Подтверждение", parameters, options);
                var result = await resultDialog.Result;

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
                    var accountReloadRequest = new SignalGlobalRequest
                    {
                        OnReloadAccountRequest = new OnReloadAccountRequest { AdditionalAccountId = recipient.Id }
                    };
                    await CurrentState.SignalRServerAsync(accountReloadRequest);

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

                var parameters = new DialogParameters<ConfirmDialog>
                {
                    { x => x.ContentText, "Вы хотите прекратить дружбу?" },
                    { x => x.ButtonText, "Да" },
                    { x => x.Color, Color.Error }
                };
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
                var resultDialog = await _dialogService.ShowAsync<ConfirmDialog>($"Подтверждение", parameters, options);
                var result = await resultDialog.Result;

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
                    var accountReloadRequest = new SignalGlobalRequest
                    {
                        OnReloadAccountRequest = new OnReloadAccountRequest
                        {
                            AdditionalAccountId = recipient.Id
                        }
                    };
                    await CurrentState.SignalRServerAsync(accountReloadRequest);

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


        /// <summary>
        /// Подтверждение удаления всей переписки
        /// </summary>
        public async Task DeleteAllMessagesDialogAsync(List<LastMessagesForAccountSpDto> lastMessagesList, int messageId)
        {
            if (CurrentState.Account != null)
            {
                var parameters = new DialogParameters<ConfirmDialog>
                {
                    { x => x.ContentText, $"Удалить всю переписку?" },
                    { x => x.ButtonText, "Удалить" },
                    { x => x.Color, Color.Error }
                };
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
                var resultDialog = await _dialogService.ShowAsync<ConfirmDialog>($"Подтверждение", parameters, options);
                var result = await resultDialog.Result;

                if (result != null && result.Canceled == false)
                {
                    var index = lastMessagesList.FindIndex(x => x.Id == messageId);
                    var recipientId = lastMessagesList[index].Sender?.Id == CurrentState.Account.Id ? lastMessagesList[index].Recipient?.Id : lastMessagesList[index].Sender?.Id;

                    if (index >= 0 && lastMessagesList[index].Sender != null && recipientId != null)
                    {
                        // 1. Удалим все сообщения в таблице Messages
                        var _deleteMessages = _serviceProvider.GetService<IRepository<DeleteMessagesRequestDto, ResponseDtoBase>>()!;
                        var apiResponse = await _deleteMessages.HttpPostAsync(new DeleteMessagesRequestDto
                        {
                            DeleteAll = true,
                            RecipientId = recipientId.Value,
                            Token = CurrentState.Account.Token
                        });

                        // 2. Добавим информационное сообщение в Messages
                        var _addMessages = _serviceProvider.GetService<IRepository<AddMessageRequestDto, AddMessageResponseDto>>()!;
                        var apiResult = await _addMessages.HttpPostAsync(new AddMessageRequestDto
                        {
                            Type = EnumMessages.AllMessagesDeleted,
                            RecipientId = recipientId.Value,
                            Text = StaticData.NotificationTypes[EnumMessages.AllMessagesDeleted].Text,
                            Token = CurrentState.Account.Token
                        });

                        // 3. Сообщим обоим пользователям о событии
                        var onMessagesUpdatedRequest = new SignalGlobalRequest
                        {
                            OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                            {
                                DeleteMessages = true,
                                RecipientId = recipientId.Value,
                                MessagesIds = null
                            }
                        };
                        await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                    }
                }
            }
        }


        #region /// БЛОКИРОВКА ПОЛЬЗОВАТЕЛЯ ///
        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        public async Task BlockAccountDialogAsync(AccountsViewDto? account1, AccountsViewDto? account2)
        {
            if (CurrentState.Account != null && account1 != null && account2 != null)
            {
                var hasBlockRelation = CurrentState.Account.Relations?.GetRelationInfo(EnumRelations.Blocked, account1, account2);

                var parameters = new DialogParameters<ConfirmDialog>
                {
                    { x => x.ContentText, hasBlockRelation == null ? $"Заблокировать пользователя?" : $"Разблокировать пользователя?" },
                    { x => x.ButtonText, hasBlockRelation == null ? "Заблокировать" : "Разблокировать" },
                    { x => x.Color, hasBlockRelation == null ? Color.Error : Color.Success }
                };
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
                var resultDialog = await _dialogService.ShowAsync<ConfirmDialog>($"Подтверждение", parameters, options);
                var result = await resultDialog.Result;

                if (result != null && result.Canceled == false)
                {
                    var recipientId = account1.Id == CurrentState.Account.Id ? account2.Id : account1.Id;
                    await BlockAccountAsync(recipientId);
                }
            }
        }

        async Task BlockAccountAsync(int recipientId)
        {
            var _repoUpdateRelation = _serviceProvider.GetService<IRepository<UpdateRelationRequestDto, UpdateRelationResponseDto>>()!;
            // Добавим связь блокировки в БД
            var apiUpdateResponse = await _repoUpdateRelation.HttpPostAsync(new UpdateRelationRequestDto
            {
                RecipientId = recipientId,
                EnumRelation = EnumRelations.Blocked,
                Token = CurrentState.Account?.Token
            });

            // Обновим состояния у обоих пользователей
            var accountReloadRequest = new SignalGlobalRequest
            {
                OnReloadAccountRequest = new OnReloadAccountRequest
                {
                    AdditionalAccountId = recipientId
                }
            };
            await CurrentState.SignalRServerAsync(accountReloadRequest);

            // Сообщим обоим пользователям о блокировке
            var onMessagesUpdatedRequest = new SignalGlobalRequest { OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest { RecipientId = recipientId } };
            if (apiUpdateResponse.Response.IsRelationAdded)
                onMessagesUpdatedRequest.OnMessagesUpdatedRequest.BlockAccount = true;
            else
                onMessagesUpdatedRequest.OnMessagesUpdatedRequest.UnblockAccount = true;
            await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
        }
        #endregion

    }
}
