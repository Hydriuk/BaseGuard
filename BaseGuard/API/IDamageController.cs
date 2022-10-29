#if OPENMOD
using OpenMod.API.Ioc;
#endif
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IDamageController
    {
        ushort ReduceDamage(ushort damage, uint buildableInstanceId, Vector3 position, CSteamID playerId, CSteamID groupId);
    }
}
