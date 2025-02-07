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
        [Inject] IRepository<DeleteMessageRequestDto, ResponseDtoBase> _repoDeleteMessage { get; set; } = null!;
        [Inject] IRepository<UpdateRelationRequestDto, ResponseDtoBase> _repoUpdateRelation { get; set; } = null!;
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
                    // Добавление сообщения в диалог двух пользователей в MessagesDialog
                    if (response.AppendNewMessages)
                        await _JSModule.InvokeVoidAsync("AppendNewMessages", await GetNewMessages());

                    // Обновление или удаление сообщения в диалогах двух пользователей в MessagesDialog
                    if (response.UpdateMessage && response.MessageId.HasValue)
                        await _JSModule.InvokeVoidAsync("UpdateMessage", response.MessageId);

                    // Пометить сообщения как прочитанные
                    if (response.MarkMessagesAsRead)
                    {
                        // Если MessagesIds = null, то помечаем прочитанными все сообщения
                        if (response.MessagesIds == null)
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
                MarkAsRead = true,
                Take = StaticData.MESSAGES_PER_BLOCK,
                Token = CurrentState.Account?.Token
            };
            var apiResponse = await _repoGetMessages.HttpPostAsync(request);
            messages.InsertRange(0, apiResponse.Response.Messages);

            // Пометим сообщения как прочитанные в MessageDialog
            var messagesIds = apiResponse.Response.Messages.Where(x => x.SenderId == Recipient.Id).Select(s => s.Id);
            if (messagesIds.Count() > 0)
            {
                var onMessagesUpdatedRequest = new SignalGlobalRequest { OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest { MarkMessagesAsRead = true, RecipientId = Recipient.Id, MessagesIds = messagesIds } };
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
                MarkAsRead = true,
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
        /// Принимаем дружбу
        /// </summary>
        [JSInvokable]
        public async Task AcceptFriendshipAsync(int messageId)
        {
            // Обновим данные о дружбе в БД
            var apiUpdateResponse = await _repoUpdateRelation.HttpPostAsync(new UpdateRelationRequestDto 
            {
                RecipientId = Recipient.Id,
                EnumRelation = EnumRelations.Friend, 
                Token = CurrentState.Account?.Token 
            });

            // Удалим сообщение о запросе дружбы из БД
            var apiDeleteResponse = await _repoDeleteMessage.HttpPostAsync(new DeleteMessageRequestDto
            {
                MessageId = messageId,
                RecipientId = Recipient.Id,
                Token = CurrentState.Account?.Token
            });

            // Добавим сообщение обоим пользователям о принятии дружбы
            text = StaticData.NotificationTypes[EnumMessages.RequestForFrendshipAccepted].Text;
            await SubmitMessageAsync(EnumMessages.RequestForFrendshipAccepted);

            // Удалим сообщение с запросом дружбы из UI у обоих пользователей
            var onMessagesUpdatedRequest = new SignalGlobalRequest
            {
                OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                {
                    UpdateMessage = true,
                    MessageId = messageId,
                    RecipientId = Recipient.Id
                }
            };
            await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);

            // Обновим состояние у обоих пользователей
            var reloadAccountRequest = new SignalGlobalRequest { OnReloadAccount = new OnReloadAccount { AdditionalAccountId = Recipient.Id } };
            await CurrentState.SignalRServerAsync(reloadAccountRequest);
        }

        [JSInvokable]
        public async Task DeclineFriendshipAsync(int messageId)
        {
            // Удалим сообщение из БД
            var request = new DeleteMessageRequestDto
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
                    UpdateMessage = true,
                    MessageId = messageId,
                    RecipientId = Recipient.Id
                }
            };
            await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
        }

        [JSInvokable]
        public async Task CancelFriendshipAsync(int messageId)
        {
            // Удалим сообщение из БД
            var request = new DeleteMessageRequestDto
            {
                MessageId = messageId,
                RecipientId = Recipient.Id,
                Token = CurrentState.Account?.Token
            };
            var apiResponse = await _repoDeleteMessage.HttpPostAsync(request);

            // Удалим сообщение с запросом дружбы из UI у обоих пользователей
            var onMessagesUpdatedRequest = new SignalGlobalRequest
            {
                OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                {
                    UpdateMessage = true,
                    MessageId = messageId,
                    RecipientId = Recipient.Id
                }
            };
            await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
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
