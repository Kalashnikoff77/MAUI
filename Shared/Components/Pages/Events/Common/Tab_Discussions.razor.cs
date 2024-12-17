using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Components;
using Data.State;

namespace Shared.Components.Pages.Events.Common
{
    public partial class Tab_Discussions : IDisposable
    {
        [Inject] CurrentState CurrentState { get; set; } = null!;
        [Parameter, EditorRequired] public SchedulesForEventsViewDto ScheduleForEventView { get; set; } = null!;
        [Inject] IRepository<GetDiscussionsForEventsRequestDto, GetDiscussionsForEventsResponseDto> _repoGetDiscussions { get; set; } = null!;
        [Inject] IRepository<AddDiscussionsForEventsRequestDto, AddDiscussionsForEventsResponseDto> _repoAddDiscussion { get; set; } = null!;
        [Inject] IJSProcessor _JSProcessor { get; set; } = null!;

        List<DiscussionsForEventsViewDto> discussions = new List<DiscussionsForEventsViewDto>();
        string? _text { get; set; } = null!;
        bool isDiscussionsLoaded;
        bool _sending;
        int _currentElementId = 0;
        bool moreDiscussionsButton = false;

        IDisposable? OnScheduleChangedHandler;

        protected override async Task OnInitializedAsync() =>
            await GetDiscussionsAsync();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                await _JSProcessor.ScrollToElementWithinDiv($"id_{_currentElementId}", "DivDiscussionsFrame");

            //OnScheduleChangedHandler = OnScheduleChangedHandler.SignalRClient<OnScheduleChangedResponse>(CurrentState, async (response) =>
            //{
            //    var responseApi = await _repoGetDiscussions.HttpPostAsync(new GetDiscussionsForEventsRequestDto()
            //    {
            //        EventId = ScheduleForEventView.EventId,
            //        GetNextAfterId = discussions.Count > 0 ? discussions.Max(m => m.Id) : null,
            //        Take = StaticData.EVENT_DISCUSSIONS_PER_BLOCK
            //    });
            //    discussions.AddRange(responseApi.Response.Discussions);

            //    moreDiscussionsButton = discussions.Count < responseApi.Response.NumOfDiscussions;

            //    _currentElementId = discussions.Any() ? discussions.Max(m => m.Id) : 0;

            //    await InvokeAsync(StateHasChanged);
            //});
        }

        async Task GetDiscussionsAsync()
        {
            var response = await _repoGetDiscussions.HttpPostAsync(new GetDiscussionsForEventsRequestDto()
            {
                EventId = ScheduleForEventView.EventId,
                GetPreviousFromId = discussions.Count > 0 ? discussions.Min(m => m.Id) : null,
                Take = StaticData.EVENT_DISCUSSIONS_PER_BLOCK
            });
            discussions.InsertRange(0, response.Response.Discussions);
            
            isDiscussionsLoaded = true;
            _currentElementId = response.Response.Discussions.Any() ? response.Response.Discussions.Max(m => m.Id) : 0;
            moreDiscussionsButton = discussions.Count < response.Response.NumOfDiscussions;
        }


        async Task OnDiscussionAdded()
        {
            if (!string.IsNullOrWhiteSpace(_text))
            {
                _sending = true;

                var responseAdd = await _repoAddDiscussion.HttpPostAsync(new AddDiscussionsForEventsRequestDto()
                {
                    Token = CurrentState.Account!.Token,
                    EventId = ScheduleForEventView.EventId,
                    Text = _text
                });

                //var request = new SignalGlobalRequest
                //{
                //    OnScheduleChanged = new OnScheduleChanged { EventId = ScheduleForEventView.EventId, ScheduleId = ScheduleForEventView.Id }
                //};
                //await CurrentState.SignalRServerAsync(request);

                _text = null;
                _sending = false;
            }
        }

        public void Dispose() =>
            OnScheduleChangedHandler?.Dispose();
    }
}
