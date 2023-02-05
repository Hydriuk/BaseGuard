#if OPENMOD
using OpenMod.API.Ioc;
#endif
using Steamworks;
using UnityEngine;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IDamageController
    {
        /// <summary>
        /// Reduce damage of a buildable
        /// </summary>
        /// <param name="damage"> Base damage amount </param>
        /// <param name="assetId"> Asset id of the buildable </param>
        /// <param name="buildableInstanceId"> Instance id of the buildable </param>
        /// <param name="playerId"> Id of the buildable's owner </param>
        /// <param name="groupId"> Id of the buildable's group </param>
        /// <returns> The reduced damage </returns>
        ushort ReduceDamage(ushort damage, ushort assetId, uint buildableInstanceId, CSteamID playerId, CSteamID groupId, CSteamID instigatorId);
    }
}