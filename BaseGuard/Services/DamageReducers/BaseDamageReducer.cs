using BaseGuard.API;
using BaseGuard.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaseGuard.Services.DamageReducers
{
    public class BaseDamageReducer : IDamageReducer
    {
        private readonly float _baseShield;

        public BaseDamageReducer(IConfigurationProvider configuration)
        {
            _baseShield = configuration.BaseShield;
        }

        public virtual float ReduceDamage(ushort damage, uint buildableInstanceId, Vector3 position) => damage * (1 - _baseShield);
    }
}
