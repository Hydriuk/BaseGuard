﻿using BaseGuard.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Connections.Events;
using SDG.Unturned;
using System.Linq;
using System.Threading.Tasks;

namespace BaseGuard.OpenMod.Events
{
    public class PlayerDisconnectedEvent : IEventListener<UnturnedPlayerDisconnectedEvent>
    {
        private readonly IProtectionDecayProvider _protectionDecayProvider;
        private readonly IGroupHistoryStore _groupHistoryStore;

        public PlayerDisconnectedEvent(IProtectionDecayProvider protectionDecayProvider, IGroupHistoryStore groupHistoryStore)
        {
            _protectionDecayProvider = protectionDecayProvider;
            _groupHistoryStore = groupHistoryStore;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerDisconnectedEvent @event)
        {
            _protectionDecayProvider.StartTimer(@event.Player.SteamId);

            if (!Provider.clients.Any(sPlayer => sPlayer.player.quests.groupID == @event.Player.Player.quests.groupID))
            {
                _protectionDecayProvider.StartTimer(@event.Player.Player.quests.groupID);
            }

            var groupHistory = _groupHistoryStore.GetPlayerGroups(@event.Player.SteamId);
            var notConnectedGroups = groupHistory.Where(group => !Provider.clients.Any(sPlayer => sPlayer.player.quests.groupID == group));

            foreach (var group in notConnectedGroups)
            {
                _protectionDecayProvider.StartTimer(group);
            }

            return Task.CompletedTask;
        }
    }
}