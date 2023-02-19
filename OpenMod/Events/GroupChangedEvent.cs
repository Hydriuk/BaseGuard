using BaseGuard.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Quests.Events;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseGuard.OpenMod.Events
{
    public class GroupChangedEvent : IEventListener<UnturnedPlayerGroupChangedEvent>
    {
        private readonly IGroupHistoryStore _groupHistoryStore;

        public GroupChangedEvent(IGroupHistoryStore groupHistoryStore)
        {
            _groupHistoryStore = groupHistoryStore;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerGroupChangedEvent @event)
        {
            if (@event.OldGroupId != CSteamID.Nil)
            {
                _groupHistoryStore.OnGroupQuit(@event.Player.SteamId, @event.OldGroupId);
            }

            return Task.CompletedTask;
        }
    }
}
