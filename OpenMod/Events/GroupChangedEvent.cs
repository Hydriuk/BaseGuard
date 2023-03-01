using BaseGuard.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Quests.Events;
using SDG.Unturned;
using Steamworks;
using System.Linq;
using System.Threading.Tasks;

namespace BaseGuard.OpenMod.Events
{
    public class GroupChangedEvent : IEventListener<UnturnedPlayerGroupChangedEvent>
    {
        private readonly IGroupHistoryStore _groupHistoryStore;
        private readonly IProtectionDecayProvider _protectionDecayProvider;

        public GroupChangedEvent(IGroupHistoryStore groupHistoryStore, IProtectionDecayProvider protectionDecayProvider)
        {
            _groupHistoryStore = groupHistoryStore;
            _protectionDecayProvider = protectionDecayProvider;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerGroupChangedEvent @event)
        {
            if (@event.OldGroupId != CSteamID.Nil)
            {
                _groupHistoryStore.OnGroupQuit(@event.Player.SteamId, @event.OldGroupId);
                _protectionDecayProvider.DestroyTimer(@event.Player.Player.quests.groupID);

                if (!Provider.clients.Any(sPlayer => sPlayer.player.quests.groupID == @event.OldGroupId))
                {
                    _protectionDecayProvider.StartTimer(@event.OldGroupId);
                }

                var groupHistory = _groupHistoryStore.GetPlayerGroups(@event.Player.SteamId);
                var notConnectedGroups = groupHistory.Where(group => !Provider.clients.Any(sPlayer => sPlayer.player.quests.groupID == group));

                foreach (var group in notConnectedGroups)
                {
                    _protectionDecayProvider.StartTimer(group);
                }
            }

            return Task.CompletedTask;
        }
    }
}