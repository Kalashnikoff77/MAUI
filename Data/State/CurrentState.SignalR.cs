﻿using Data.Models.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace Data.State
{
    public partial class CurrentState : IDisposable
    {
        public HubConnection? SignalR { get; set; }
        public HashSet<string> ConnectedAccounts { get; set; } = new();

        IDisposable? updateOnlineAccountsHandler;
        IDisposable? onAvatarChangedHandler;

        //IDisposable? updateRelationsTriggerHandler;
        //IDisposable? updateEventRegisterTriggerHandler;

        public async Task SignalRConnect()
        {
            try
            {
                await SignalRDisconnect();

                string? host;
                if (_formFactor.GetFormFactor() == "Phone")
                    host = _config.GetRequiredSection("SignalR:AndroidHost").Value;
                else
                    host = _config.GetRequiredSection("SignalR:WinHost").Value;

                SignalR = new HubConnectionBuilder()
                    .WithUrl(_navigationManager.ToAbsoluteUri(host), (c) => { c.AccessTokenProvider = () => Task.FromResult(Account?.Token); })
                    .WithAutomaticReconnect()
                    .WithServerTimeout(TimeSpan.FromHours(24))
                    .Build();

                // Пользователь подключился
                updateOnlineAccountsHandler = updateOnlineAccountsHandler.SignalRClient<OnAccountConnectedResponse>(this);

                //// Пользователь сменил аватар
                onAvatarChangedHandler = onAvatarChangedHandler.SignalRClient<OnAvatarChangedResponse>(this);

                //// Пользователь изменил взаимоотношения с другим (дружба, подписка, блокировка)
                //updateRelationsTriggerHandler = updateRelationsTriggerHandler.SignalRClient<GetRelationsModel>(this);

                await SignalR.StartAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public async Task SignalRDisconnect()
        {
            if (SignalR != null)
            {
                await SignalR.DisposeAsync();
                SignalR = null;
            }
        }

        public void Dispose()
        {
            updateOnlineAccountsHandler?.Dispose();
            onAvatarChangedHandler?.Dispose();

            //updateRelationsTriggerHandler?.Dispose();
            //updateEventRegisterTriggerHandler?.Dispose();
        }
    }
}