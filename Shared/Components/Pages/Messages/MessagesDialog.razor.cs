using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Enums;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Shared.Components.Dialogs;
using Shared.Components.Pages.Messages.OneMessage;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Components.Pages.Messages
{
    public partial class MessagesDialog : IAsyncDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;
        [Parameter, Required] public AccountsViewDto Recipient { get; set; } = null!;

        [Inject] IRepository<GetMessagesRequestDto, GetMessagesResponseDto> _repoGetMessages { get; set; } = null!;
        [Inject] IRepository<AddMessageRequestDto, AddMessageResponseDto> _repoAddMessage { get; set; } = null!;
        [Inject] IRepository<DeleteMessagesRequestDto, ResponseDtoBase> _repoDeleteMessage { get; set; } = null!;
        [Inject] IRepository<UpdateRelationRequestDto, UpdateRelationResponseDto> _repoUpdateRelation { get; set; } = null!;
        [Inject] IRepository<DeleteRelationRequestDto, ResponseDtoBase> _repoDeleteRelation { get; set; } = null!;
        [Inject] ShowDialogs _showDialogs { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;
        [Inject] IComponentRenderer<BaseMessage> _renderer { get; set; } = null!;

        DotNetObjectReference<MessagesDialog> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;

        IDisposable? OnMessagesUpdatedHandler;

        List<MessagesDto> messages = new List<MessagesDto>();

        string? text;
        bool sending;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                OnMessagesUpdatedHandler = OnMessagesUpdatedHandler.SignalRClient<OnMessagesUpdatedResponse>(CurrentState, async (response) =>
                {
                    // Добавление новых сообщений в диалог двух пользователей в MessagesDialog
                    if (response.AppendNewMessages)
                        await _JSModule.InvokeVoidAsync("AppendNewMessages", await GetNewMessages());

                    // Обновление или удаление сообщения в диалогах двух пользователей в MessagesDialog
                    if (response.UpdateMessage && response.MessageId.HasValue)
                        await _JSModule.InvokeVoidAsync("UpdateMessage", response.MessageId);

                    // Пометить одно или несколько сообщений как прочитанные
                    if (response.MarkMessagesAsRead && response.MessagesIds != null)
                    {
                        foreach (var messageId in response.MessagesIds)
                        {
                            var index = messages.FindIndex(x => x.Id == messageId && x.ReadDate == null);
                            if (index >= 0)
                            {
                                messages[index].ReadDate = DateTime.Now;
                                var html = await _renderer.RenderAsync(new Dictionary<string, object?> { { "AccountId", CurrentState.Account?.Id }, { "Message", messages[index] } });
                                await _JSModule.InvokeVoidAsync("MarkMessageAsRead", messageId, html);
                            }
                        }
                    }

                    // Пометить все сообщения как прочитанные
                    if (response.MarkAllMessagesAsRead)
                    {
                        response.MessagesIds = messages.Where(x => x.RecipientId == Recipient.Id && x.ReadDate == null).Select(x => x.Id);

                        foreach (var messageId in response.MessagesIds)
                        {
                            var index = messages.FindIndex(x => x.Id == messageId && x.ReadDate == null);
                            if (index >= 0)
                            {
                                messages[index].ReadDate = DateTime.Now;
                                var html = await _renderer.RenderAsync(new Dictionary<string, object?> { { "AccountId", CurrentState.Account?.Id }, { "Message", messages[index] } });
                                await _JSModule.InvokeVoidAsync("MarkMessageAsRead", messageId, html);
                            }
                        }
                    }

                    // Удаление всей переписки в диалогах двух пользователей в MessagesDialog
                    if (response.DeleteMessages)
                    {
                        // Удалим все сообщения в MessagesDialog
                        await _JSModule.InvokeVoidAsync("DeleteMessages");

                        var onMessagesUpdatedRequest = new SignalGlobalRequest
                        {
                            OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                            {
                                AppendNewMessages = true,
                                RecipientId = Recipient.Id
                            }
                        };
                        await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                        await InvokeAsync(StateHasChanged);
                    }

                    // Блокировка пользователя
                    if (response.BlockAccount)
                    {
                        var onMessagesUpdatedRequest = new SignalGlobalRequest
                        {
                            OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                            {
                                AppendNewMessages = true,
                                RecipientId = Recipient.Id
                            }
                        };
                        await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                        await InvokeAsync(StateHasChanged);
                    }

                    // Разблокировка пользователя
                    if (response.UnblockAccount)
                    {
                        var onMessagesUpdatedRequest = new SignalGlobalRequest
                        {
                            OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                            {
                                AppendNewMessages = true,
                                RecipientId = Recipient.Id
                            }
                        };
                        await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                        await InvokeAsync(StateHasChanged);
                    }
                });

                _dotNetReference = DotNetObjectReference.Create(this);
                _JSModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/Pages/Messages/MessagesScroll.js");
                await _JSModule.InvokeVoidAsync("Initialize", _dotNetReference);
                await _JSModule.InvokeVoidAsync("LoadItems");
                await _JSModule.InvokeVoidAsync("ScrollDivToBottom");
            }
        }

        [JSInvokable]
        public async Task<string> LoadItemsAsync()
        {
            var request = new GetMessagesRequestDto
            {
                RecipientId = Recipient.Id,
                GetPreviousFromId = messages.Count > 0 ? messages.Min(m => m.Id) : null,
                Take = StaticData.MESSAGES_PER_BLOCK,
                Token = CurrentState.Account?.Token
            };
            var apiResponse = await _repoGetMessages.HttpPostAsync(request);
            messages.InsertRange(0, apiResponse.Response.Messages);

            // Пометим сообщения как прочитанные в MessageDialog
            var messagesIds = apiResponse.Response.Messages.Where(x => x.SenderId == Recipient.Id).Select(s => s.Id);
            if (messagesIds.Count() > 0)
            {
                var onMessagesUpdatedRequest = new SignalGlobalRequest
                {
                    OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                    {
                        MarkMessagesAsRead = true,
                        RecipientId = Recipient.Id,
                        MessagesIds = messagesIds
                    }
                };
                await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
            }

            // Ручная генерация компонентов сообщений
            return await RenderMessages(apiResponse.Response.Messages);
        }

        public async Task<string> GetNewMessages()
        {
            var request = new GetMessagesRequestDto
            {
                RecipientId = Recipient.Id,
                GetNextAfterId = messages.Count > 0 ? messages.Max(m => m.Id) : null,
                Take = StaticData.MESSAGES_PER_BLOCK,
                Token = CurrentState.Account?.Token
            };
            var apiResponse = await _repoGetMessages.HttpPostAsync(request);
            messages.AddRange(apiResponse.Response.Messages);

            // Пометим сообщение как прочитанное в MessageDialog
            var messagesIds = apiResponse.Response.Messages.Where(x => x.SenderId == Recipient.Id).Select(s => s.Id);
            if (messagesIds.Count() > 0)
            {
                var onMessagesUpdatedRequest = new SignalGlobalRequest { OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest { MarkMessagesAsRead = true, RecipientId = Recipient.Id, MessagesIds = messagesIds } };
                await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
            }

            // Ручная генерация компонентов сообщений
            return await RenderMessages(apiResponse.Response.Messages);
        }

        /// <summary>
        /// Ручная генерация компонента BaseMessage (сообщения пользователя)
        /// </summary>
        async Task<string> RenderMessages(List<MessagesDto> messages)
        {
            var html = new StringBuilder(5000);

            foreach (var message in messages)
                html.Append(await _renderer.RenderAsync(new Dictionary<string, object?> { { "AccountId", CurrentState.Account?.Id }, { "Message", message } }));

            return html.ToString();
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        async Task SubmitMessageAsync(EnumMessages type = EnumMessages.Message)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                sending = true;
                var response = await _repoAddMessage.HttpPostAsync(new AddMessageRequestDto
                {
                    Type = type,
                    RecipientId = Recipient.Id,
                    Text = text,
                    Token = CurrentState.Account?.Token
                });

                var onMessagesUpdatedRequest = new SignalGlobalRequest 
                {
                    OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest 
                    {
                        AppendNewMessages = true,
                        RecipientId = Recipient.Id
                    } 
                };
                await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);

                text = null;
                sending = false;
            }
        }

        /// <summary>
        /// Принятие дружбы
        /// </summary>
        [JSInvokable]
        public async Task AcceptFriendshipRequestAsync(int messageId) =>
            await _showDialogs.AcceptFriendshipRequestDialogAsync(CurrentState.Account, Recipient);

        /// <summary>
        /// Отказ принятия дружбы
        /// </summary>
        [JSInvokable]
        public async Task DeclineFriendshipRequestAsync(int messageId)
        {
            await _showDialogs.DeclineFriendshipRequestDialogAsync(CurrentState.Account, Recipient);

            return;

            // Удалим сообщение из БД
            var request = new DeleteMessagesRequestDto
            {
                MessageId = messageId,
                RecipientId = Recipient.Id,
                Token = CurrentState.Account?.Token
            };
            var apiResponse = await _repoDeleteMessage.HttpPostAsync(request);

            text = StaticData.NotificationTypes[EnumMessages.RequestForFrendshipDeclined].Text;
            await SubmitMessageAsync(EnumMessages.RequestForFrendshipDeclined);

            // Удалим сообщение с запросом дружбы из UI у обоих пользователей
            var onMessagesUpdatedRequest = new SignalGlobalRequest
            {
                OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                {
                    DeleteMessage = true,
                    MessageId = messageId,
                    RecipientId = Recipient.Id
                }
            };
            await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
        }

        /// <summary>
        /// Отмена запроса инициатором на добавление в друзья
        /// </summary>
        [JSInvokable]
        public async Task AbortFriendshipRequestAsync(int messageId)
        {
            await _showDialogs.AbortFriendshipRequestDialogAsync(CurrentState.Account, Recipient);

            return;

            ////ShowDialogs.FriendshipDialogAsync(CurrentState.Account, Account);

            //// Удалим данные о запросе дружбы в БД
            //var apiUpdateResponse = await _repoDeleteRelation.HttpPostAsync(new DeleteRelationRequestDto
            //{
            //    RecipientId = Recipient.Id,
            //    EnumRelation = EnumRelations.Friend,
            //    Token = CurrentState.Account?.Token
            //});

            //// Удалим сообщение с запросом дружбы из БД и UI у обоих пользователей
            //var onMessagesUpdatedRequest = new SignalGlobalRequest
            //{
            //    OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
            //    {
            //        DeleteMessage = true,
            //        MessageId = messageId,
            //        RecipientId = Recipient.Id
            //    }
            //};
            //await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                OnMessagesUpdatedHandler?.Dispose();
                _dotNetReference?.Dispose();

                if (_JSModule != null)
                    await _JSModule.DisposeAsync();
            }
            // Отлов ошибки, когда соединение SignalR разорвано, но производится попытка вызвать JS. Возникает при перезагрузке страницы (F5)
            catch (JSDisconnectedException ex)
            {
            }
        }
    }
}
