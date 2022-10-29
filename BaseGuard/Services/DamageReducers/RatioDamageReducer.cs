using BaseGuard.API;
using BaseGuard.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaseGuard.Services.DamageReducers
{
    public class RatioDamageReducer : BaseDamageReducer, IDamageReducer
    {
        public RatioDamageReducer(IConfigurationProvider configuration) : base(configuration)
        {
        }

        public float ReduceDamage(ushort damage, uint buildableInstanceId, Vector3 position)
        {
            throw new NotImplementedException();
        }
    }
}
