using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Data.State;
using Data.Models.SignalR;

namespace Shared.Components.Pages.Events
{
    public partial class RegisterForScheduleDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;
        [CascadingParameter] CurrentState CurrentState { get; set; } = null!;
        [Parameter, EditorRequired] public SchedulesForEventsViewDto ScheduleForEventView { get; set; } = null!;
        [Parameter, EditorRequired] public bool IsRegistered { get; set; }

        [Inject] IRepository<EventRegistrationRequestDto, EventRegistrationResponseDto> _repoUpdateRegistration { get; set; } = null!;

        async Task Submit()
        {
            if (CurrentState.Account != null)
            {
                MudDialog.Close(DialogResult.Ok(true));

                var apiResponse = await _repoUpdateRegistration.HttpPostAsync(new EventRegistrationRequestDto
                {
                    Token = CurrentState.Account.Token,
                    ScheduleId = ScheduleForEventView.Id
                });

                await CurrentState.ReloadAccountAsync();

                var request = new SignalGlobalRequest
                {
                    OnScheduleUpdatedRequest = new OnScheduleUpdatedRequest { EventId = ScheduleForEventView.EventId, ScheduleId = ScheduleForEventView.Id }
                };
                await CurrentState.SignalRServerAsync(request);
            }
        }

        void Cancel() => MudDialog.Cancel();
    }
}
