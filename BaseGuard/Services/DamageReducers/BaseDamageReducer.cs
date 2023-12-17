using BaseGuard.API;
using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using System.Collections.Generic;

namespace BaseGuard.Services.DamageReducers
{
    public class BaseDamageReducer : IDamageReducer
    {
        private readonly float _baseShield;
        private readonly bool _prioritizeOverrides;

        private readonly Dictionary<ushort, ShieldOverride> _shieldOverrides = new Dictionary<ushort, ShieldOverride>();

        private readonly IProtectionScheduler _protectionScheduler;

        public BaseDamageReducer(
            IConfigurationAdapter<Configuration> confAdapter,
            IProtectionScheduler protectionScheduler)
        {
            _baseShield = confAdapter.Configuration.BaseShield;
            _prioritizeOverrides = confAdapter.Configuration.PrioritizeOverrides;

            _protectionScheduler = protectionScheduler;

            foreach (var shieldOverride in confAdapter.Configuration.Overrides)
            {
                foreach (var id in shieldOverride.Ids)
                {
                    _shieldOverrides.Add(id, shieldOverride);
                }
            }
        }

        public virtual float ReduceDamage(ushort damage, ushort assetId, uint buildableInstanceId)
        {
            if(_shieldOverrides.TryGetValue(assetId, out ShieldOverride shieldOverride))
            {
                if(_prioritizeOverrides || _protectionScheduler.IsActive)
                {
                    return damage * (1 - shieldOverride.BaseShield);
                }
            }

            if (_protectionScheduler.IsActive)
            {
                return damage * (1 - _baseShield);
            }
            else
            {
                return damage;
            }
        }
    }
}