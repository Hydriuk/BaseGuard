using BaseGuard.Models;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGuard.API
{
    public interface IDamageReducer
    {
        float ReduceDamage(ushort damage, ushort assetId, uint buildableInstanceId, Vector3 position);
    }
}
