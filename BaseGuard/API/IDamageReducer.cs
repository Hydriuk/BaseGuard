using BaseGuard.Models;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGuard.API
{
    public interface IDamageReducer
    {
        float ReduceDamage(float damage, uint buildableInstanceId, Vector3 position);
    }
}
