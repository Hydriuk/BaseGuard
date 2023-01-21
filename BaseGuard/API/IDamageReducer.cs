using UnityEngine;

namespace BaseGuard.API
{
    public interface IDamageReducer
    {
        /// <summary>
        /// Reduce damage of a buildable
        /// </summary>
        /// <param name="damage"> Base damage amount </param>
        /// <param name="assetId"> Asset id of the buildable </param>
        /// <param name="buildableInstanceId"> Instance id of the buildable </param>
        /// <returns> The reduced damage </returns>
        float ReduceDamage(ushort damage, ushort assetId, uint buildableInstanceId);
    }
}