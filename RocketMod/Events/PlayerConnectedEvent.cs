using BaseGuard.API;
using SDG.Unturned;
using System;

namespace BaseGuard.RocketMod.Events
{
    public class PlayerConnectedEvent : IDisposable
    {
        private readonly IProtectionDecayProvider _protectionDecayProvider;
        private readonly IGroupHistoryStore _groupHistoryStore;

        public PlayerConnectedEvent(IProtectionDecayProvider protectionDecayProvider, IGroupHistoryStore groupHistoryStore)
        {
            _protectionDecayProvider = protectionDecayProvider;
            _groupHistoryStore = groupHistoryStore;

            Provider.onEnemyConnected += OnPlayerConnected;
        }

        public void Dispose()
        {
            Provider.onEnemyConnected -= OnPlayerConnected;
        }

        private void OnPlayerConnected(SteamPlayer sPlayer)
        {
            _protectionDecayProvider.DestroyTimer(sPlayer.playerID.steamID);
            _protectionDecayProvider.DestroyTimer(sPlayer.player.quests.groupID);

            var groupHistory = _groupHistoryStore.GetPlayerGroups(sPlayer.playerID.steamID);
            foreach (var group in groupHistory)
            {
                _protectionDecayProvider.DestroyTimer(group);
            }
        }
    }
}