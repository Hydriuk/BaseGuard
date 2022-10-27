#if OPENMOD
using OpenMod.API.Ioc;
#endif
using Steamworks;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IActiveRaidProvider
    {
        bool TryActivateRaid(CSteamID playerId, CSteamID groupId);
        bool IsRaidActivate(CSteamID steamId);
    }
}