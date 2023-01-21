using BaseGuard.API;
using Steamworks;

namespace BaseGuard.Services.GuardActivators
{
    public class OfflineGuardActivator : IGuardActivator
    {
        private readonly IActiveRaidProvider _activeRaidProvider;

        public OfflineGuardActivator(IActiveRaidProvider activeRaidProvider)
        {
            _activeRaidProvider = activeRaidProvider;
        }

        public bool IsGuardActive(CSteamID steamID) => !_activeRaidProvider.IsRaidActivate(steamID);

        public bool TryActivateGuard(CSteamID playerId, CSteamID groupId) => !_activeRaidProvider.TryActivateRaid(playerId, groupId);
    }
}