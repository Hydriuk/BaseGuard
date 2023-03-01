using BaseGuard.API;
using SDG.Unturned;
using System;
using System.Linq;

namespace BaseGuard.RocketMod.Events
{
    public class PlayerDisconnectedEvent : IDisposable
    {
        private readonly IProtectionDecayProvider _protectionDecayProvider;
        private readonly IGroupHistoryStore _groupHistoryStore;

        public PlayerDisconnectedEvent(IProtectionDecayProvider protectionDecayProvider, IGroupHistoryStore groupHistoryStore)
        {
            _protectionDecayProvider = protectionDecayProvider;
            _groupHistoryStore = groupHistoryStore;

            Provider.onEnemyDisconnected += OnPlayerDisconnected;
        }

        public void Dispose()
        {
            Provider.onEnemyDisconnected -= OnPlayerDisconnected;
        }

        private void OnPlayerDisconnected(SteamPlayer sPlayer)
        {
            _protectionDecayProvider.StartTimer(sPlayer.playerID.steamID);

            if (!Provider.clients.Any(sPlayer => sPlayer.player.quests.groupID == sPlayer.player.quests.groupID))
            {
                _protectionDecayProvider.StartTimer(sPlayer.player.quests.groupID);
            }

            var groupHistory = _groupHistoryStore.GetPlayerGroups(sPlayer.playerID.steamID);
            var notConnectedGroups = groupHistory.Where(group => !Provider.clients.Any(sPlayer => sPlayer.player.quests.groupID == group));

            foreach (var group in notConnectedGroups)
            {
                _protectionDecayProvider.StartTimer(group);
            }
        }
    }
}