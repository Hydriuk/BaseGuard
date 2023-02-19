using BaseGuard.API;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseGuard.RocketMod.Events
{
    public class GroupChangedEvent : IDisposable
    {
        private readonly IGroupHistoryStore _groupHistoryStore;

        public GroupChangedEvent(IGroupHistoryStore groupHistoryStore)
        {
            _groupHistoryStore = groupHistoryStore;

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
            }
        }
    }
}
