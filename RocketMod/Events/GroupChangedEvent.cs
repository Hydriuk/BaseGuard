using BaseGuard.API;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;

namespace BaseGuard.RocketMod.Events
{
    public class GroupChangedEvent : IDisposable
    {
        private readonly IGroupHistoryStore _groupHistoryStore;
        private readonly IProtectionDecayProvider _protectionDecayProvider;

        public GroupChangedEvent(IGroupHistoryStore groupHistoryStore, IProtectionDecayProvider protectionDecayProvider)
        {
            _groupHistoryStore = groupHistoryStore;
            _protectionDecayProvider = protectionDecayProvider;

            PlayerQuests.onGroupChanged += OnGroupIdChanged;
        }

        public void Dispose()
        {
            PlayerQuests.onGroupChanged -= OnGroupIdChanged;
        }

        private void OnGroupIdChanged(PlayerQuests sender, CSteamID oldGroupID, EPlayerGroupRank oldGroupRank, CSteamID newGroupID, EPlayerGroupRank newGroupRank)
        {
            if (oldGroupID != CSteamID.Nil)
            {
                _groupHistoryStore.OnGroupQuit(sender.channel.owner.playerID.steamID, oldGroupID);
                _protectionDecayProvider.DestroyTimer(sender.channel.owner.playerID.steamID);

                if (!Provider.clients.Any(sPlayer => sPlayer.player.quests.groupID == oldGroupID))
                {
                    _protectionDecayProvider.StartTimer(oldGroupID);
                }

                var groupHistory = _groupHistoryStore.GetPlayerGroups(sender.channel.owner.playerID.steamID);
                var notConnectedGroups = groupHistory.Where(group => !Provider.clients.Any(sPlayer => sPlayer.player.quests.groupID == group));

                foreach (var group in notConnectedGroups)
                {
                    _protectionDecayProvider.StartTimer(group);
                }
            }
        }
    }
}