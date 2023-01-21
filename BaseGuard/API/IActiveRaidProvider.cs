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
        /// <summary>
        /// Sets raid active
        /// </summary>
        /// <param name="playerId"> Player to activate the raid of </param>
        /// <param name="groupId"> Group to activate the raid of </param>
        /// <returns></returns>
        bool TryActivateRaid(CSteamID playerId, CSteamID groupId);

        /// <summary>
        /// Checks if there is an active raid for the given id
        /// </summary>
        /// <param name="steamId"> Id to check </param>
        /// <returns> True if raid is active, false otherwise </returns>
        bool IsRaidActivate(CSteamID steamId);
    }
}