using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;

namespace Shared.Components.Pages.Events.Common
{
    public partial class Tab_Discussions : IAsyncDisposable
    {
        [Inject] CurrentState CurrentState { get; set; } = null!;
        [Parameter, EditorRequired] public SchedulesForEventsViewDto ScheduleForEventView { get; set; } = null!;
        [Inject] IRepository<GetDiscussionsForEventsRequestDto, GetDiscussionsForEventsResponseDto> _repoGetDiscussions { get; set; } = null!;
        [Inject] IRepository<AddDiscussionsForEventsRequestDto, AddDiscussionsForEventsResponseDto> _repoAddDiscussion { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;
        [Inject] IComponentRenderer<OneDiscussion> _renderer { get; set; } = null!;

        DotNetObjectReference<Tab_Discussions> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;

        List<DiscussionsForEventsViewDto> discussions = new List<DiscussionsForEventsViewDto>();
        string? text { get; set; } = null!;
        bool isDiscussionsLoaded;
        bool sending;

        IDisposable? OnScheduleChangedHandler;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                OnScheduleChangedHandler = OnScheduleChangedHandler.SignalRClient<OnScheduleChangedResponse>(CurrentState, async (response) =>
                {
                    var apiResponse = await _repoGetDiscussions.HttpPostAsync(new GetDiscussionsForEventsRequestDto()
                    {
                        EventId = ScheduleForEventView.EventId,
                        GetNextAfterId = discussions.Count > 0 ? discussions.Max(m => m.Id) : null,
                        Take = StaticData.EVENT_DISCUSSIONS_PER_BLOCK
                    });
                    discussions.AddRange(apiResponse.Response.Discussions);
                    await InvokeAsync(StateHasChanged);
                });

                _dotNetReference = DotNetObjectReference.Create(this);
                _JSModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/Pages/Events/TabDiscussionsScroll.js");
                await _JSModule.InvokeVoidAsync("GetPreviousDiscussions", _dotNetReference);
            }
        }

        [JSInvokable]
        public async Task<string> GetPreviousDiscussionsAsync()
        {
            var response = await _repoGetDiscussions.HttpPostAsync(new GetDiscussionsForEventsRequestDto()
            {
                EventId = ScheduleForEventView.EventId,
                GetPreviousFromId = discussions.Count > 0 ? discussions.Min(m => m.Id) : null,
                Take = StaticData.EVENT_DISCUSSIONS_PER_BLOCK
            });
            discussions.InsertRange(0, response.Response.Discussions);
            
            //isDiscussionsLoaded = true;
            //currentElementId = response.Response.Discussions.Any() ? response.Response.Discussions.Max(m => m.Id) : 0;
            //moreDiscussionsButton = discussions.Count < response.Response.Count;

            // Ручная генерация компонентов сообщений
            return await RenderDiscussions(response.Response.Discussions);
        }


        /// <summary>
        /// Ручная генерация компонента OneDiscussion
        /// </summary>
        async Task<string> RenderDiscussions(List<DiscussionsForEventsViewDto> discussions)
        {
            var html = new StringBuilder(5000);

            foreach (var discussion in discussions)
                html.Append(await _renderer.RenderAsync(new Dictionary<string, object?> { { "Discussion", discussion } }));

            return html.ToString();
        }


        async Task SubmitDiscussionAsync()
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                sending = true;

                var responseAdd = await _repoAddDiscussion.HttpPostAsync(new AddDiscussionsForEventsRequestDto()
                {
                    Token = CurrentState.Account!.Token,
                    EventId = ScheduleForEventView.EventId,
                    Text = text
                });

                var request = new SignalGlobalRequest
                {
                    OnScheduleChanged = new OnScheduleChanged { EventId = ScheduleForEventView.EventId, ScheduleId = ScheduleForEventView.Id }
                };
                await CurrentState.SignalRServerAsync(request);

                text = null;
                sending = false;
            }
        }


        public async ValueTask DisposeAsync()
        {
            try
            {
                OnScheduleChangedHandler?.Dispose();
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
