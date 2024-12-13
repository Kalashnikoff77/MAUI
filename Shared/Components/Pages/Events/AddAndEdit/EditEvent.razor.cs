using Common.Dto.Requests;
using Common.Models;
using MudBlazor;
using System.Net;

namespace Shared.Components.Pages.Events.AddAndEdit
{
    public partial class EditEvent : EventDtoBase, IDisposable
    {
        protected override async Task OnInitializedAsync()
        {
            var apiCountriesResponse = await _repoGetCountries.HttpPostAsync(new GetCountriesRequestDto());
            countries = apiCountriesResponse.Response.Countries;

            await ReloadEvent(EventId);

            TabPanels = new Dictionary<short, TabPanel>
            {
                { 1, new TabPanel { Items = new Dictionary<string, bool>
                    {
                        { nameof(Event.Name), true },
                        { nameof(Event.Description), true },
                        { nameof(Event.MaxPairs), true },
                        { nameof(Event.MaxMen), true },
                        { nameof(Event.MaxWomen), true },
                        { nameof(Event.Country), true },
                        { nameof(Event.Country.Region), true },
                        { nameof(Event.Address), true }
                    } }
                },
                { 2, new TabPanel { Items = new Dictionary<string, bool> { { "Schedule", true } } } },
                { 3, new TabPanel { Items = new Dictionary<string, bool> { { "Photos", true } } } }
            };

            NameIconColor = DescriptionIconColor = CountryIconColor = RegionIconColor = AddressIconColor = MaxPairsIconColor = MaxMenIconColor = MaxWomenIconColor = Color.Success;

            CheckPanelsVisibility();
        }


        async void UpdateAsync()
        {
            processingEvent = true;
            StateHasChanged();

            // Обновление мероприятия
            var request = new UpdateEventRequestDto { Event = Event, Token = CurrentState.Account?.Token };
            var apiUpdateResponse = await _repoUpdateEvent.HttpPostAsync(request);

            // Перезагрузка мероприятия
            await ReloadEvent(EventId);

            isDataSaved = true;
            processingEvent = false;
            StateHasChanged();
        }


        async Task ReloadEvent(int? eventId)
        {
            if (eventId.HasValue && eventId > 0)
            {
                var apiResponse = await _repoGetEvent.HttpPostAsync(new GetEventsRequestDto { EventId = EventId, IsPhotosIncluded = true });
                if (apiResponse.StatusCode == HttpStatusCode.OK && apiResponse.Response.Event != null)
                {
                    Event = apiResponse.Response.Event;
                    CountryText = Event.Country!.Name;
                    RegionText = Event.Country.Region.Name;

                    var apiSchedulesResponse = await _repoGetSchedules.HttpPostAsync(new GetSchedulesRequestDto { EventId = EventId });
                    Event.Schedule = apiSchedulesResponse.Response.Schedules;
                }
            }
        }
    }
}
