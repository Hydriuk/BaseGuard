using BaseGuard.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IGuardProvider
    {
        /// <summary>
        /// Get a builable guards
        /// </summary>
        /// <param name="buildableInstanceId"> Instance id of the buildable </param>
        /// <returns> One instance of each type of guards in range of the buildable </returns>
        IEnumerable<Guard> GetGuards(uint buildableInstanceId);

        /// <summary>
        /// Add a guard to the system from the thread pool. Will add the guard to all in range buildables
        /// </summary>
        /// <param name="assetId"> Asset id of the guard </param>
        /// <param name="buildableInstanceId"> Instance id of the guard </param>
        /// <param name="position"> Position of the guard </param>
        /// <param name="isActive"> Is the guard active </param>
        void AddGuardThreaded(ushort assetId, uint buildableInstanceId, Vector3 position, bool isActive);

        /// <summary>
        /// Add a buildable to the system and try to add it as a guard from the thread pool. Will look for in range guards
        /// </summary>
        /// <param name="assetId"> Asset id of the buildable </param>
        /// <param name="buildableInstanceId"> Instance id of the buildable </param>
        /// <param name="position"> Position of the buildable </param>
        /// <param name="isActive"> Is the guard buildable </param>
        void AddBuilableThreaded(ushort assetId, uint buildableInstanceId, Vector3 position, bool isActive);

        /// <summary>
        /// Removes a buildable from the system from the thread pool. Will also remove the guard if the buildable is one
        /// </summary>
        /// <param name="instanceId"> Instance id of the buildable </param>
        void RemoveBuilableThreaded(uint instanceId);

        /// <summary>
        /// Updates a guard's state from the thread pool
        /// </summary>
        /// <param name="instanceId"> Instance id of the guard </param>
        /// <param name="active"> Is the guard active </param>
        void UpdateGuardThreaded(uint instanceId, bool active);
    }
}