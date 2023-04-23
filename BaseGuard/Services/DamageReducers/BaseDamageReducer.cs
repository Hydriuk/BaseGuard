using BaseGuard.API;
using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using System.Collections.Generic;
using System.Linq;

namespace BaseGuard.Services.DamageReducers
{
    public class BaseDamageReducer : IDamageReducer
    {
        private readonly float _baseShield;

        private readonly Dictionary<ushort, ShieldOverride> _guardOverrides = new Dictionary<ushort, ShieldOverride>();

        public BaseDamageReducer(IConfigurationAdapter<Configuration> confAdapter)
        {
            _baseShield = confAdapter.Configuration.BaseShield;

            foreach (var shieldOverride in confAdapter.Configuration.Overrides)
            {
                foreach (var id in shieldOverride.Ids)
                {
                    _guardOverrides.Add(id, shieldOverride);
                }
            }
        }

        public virtual float ReduceDamage(ushort damage, ushort assetId, uint buildableInstanceId)
        {
            if (_guardOverrides.TryGetValue(assetId, out var guardOverride))
                return damage * (1 - guardOverride.BaseShield);
            else
                return damage * (1 - _baseShield);
        }
    }
}