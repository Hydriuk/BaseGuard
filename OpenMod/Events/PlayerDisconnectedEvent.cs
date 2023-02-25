using BaseGuard.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Connections.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseGuard.OpenMod.Events
{
    public class PlayerDisconnectedEvent : IEventListener<UnturnedPlayerDisconnectedEvent>
    {
        private readonly IProtectionDecayProvider _protectionDecayProvider;

        public PlayerDisconnectedEvent(IProtectionDecayProvider protectionDecayProvider)
        {
            _protectionDecayProvider = protectionDecayProvider;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerDisconnectedEvent @event)
        {
            _protectionDecayProvider.StartTimer(@event.Player.SteamId);

            return Task.CompletedTask;
        }
    }
}
