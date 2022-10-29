using BaseGuard.Models;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGuard.API
{
    public interface IDamageReducer
    {
        float ReduceDamage(ushort damage, uint buildableInstanceId, Vector3 position);
    }
}
