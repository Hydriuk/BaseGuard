using BaseGuard.API;
using Steamworks;

namespace BaseGuard.Services.GuardActivators
{
    public class PermanentGuardActivator : IGuardActivator
    {
        public bool IsGuardActive(CSteamID steamID) => true;

        public bool TryActivateGuard(CSteamID playerId, CSteamID groupId) => true;
    }
}