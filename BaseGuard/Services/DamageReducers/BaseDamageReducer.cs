using BaseGuard.API;
using BaseGuard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaseGuard.Services.DamageReducers
{
    public class BaseDamageReducer : IDamageReducer
    {
        private readonly float _baseShield;

        private readonly Dictionary<ushort, ShieldOverride> _guardOverrides;

        public BaseDamageReducer(IConfigurationProvider configuration)
        {
            _baseShield = configuration.BaseShield;
            _guardOverrides = configuration.Overrides.ToDictionary(guard => guard.Id);
        }

        public virtual float ReduceDamage(ushort damage, ushort assetId, uint buildableInstanceId, Vector3 position)
        {
            if(_guardOverrides.TryGetValue(assetId, out var guardOverride))
                return damage * (1 - guardOverride.BaseShield);
            else
                return damage * (1 - _baseShield);
        }
    }
}
