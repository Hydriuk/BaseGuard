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
    public class PlayerConnectedEvent : IEventListener<UnturnedPlayerConnectedEvent>
    {
        private readonly IProtectionDecayProvider _protectionDecayProvider;

        public PlayerConnectedEvent(IProtectionDecayProvider protectionDecayProvider)
        {
            _protectionDecayProvider = protectionDecayProvider;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerConnectedEvent @event)
        {
            _protectionDecayProvider.DestroyTimer(@event.Player.SteamId);

            return Task.CompletedTask;
        }
    }
}
