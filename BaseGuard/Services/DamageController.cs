using BaseGuard.API;
using BaseGuard.Models;
using BaseGuard.Services.DamageReducers;
using BaseGuard.Services.GuardActivators;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class DamageController : IDamageController
    {
        private readonly IGuardActivator _guardActivator;
        private readonly IDamageReducer _damageReducer;
        private readonly Dictionary<ushort, ShieldOverride> _guardOverrides;

        public DamageController(IConfigurationProvider configuration, IActiveRaidProvider activeRaidProvider, IGuardProvider guardProvider)
        {
            _guardOverrides = configuration.Overrides.ToDictionary(guard => guard.Id);

            switch (configuration.ActivationMode)
            {
                case EActivationMode.Permanent:
                    _guardActivator = new PermanentGuardActivator();
                    break;

                case EActivationMode.Offline:
                    _guardActivator = new OfflineGuardActivator(activeRaidProvider);
                    break;

                case EActivationMode.Unabled:
                default:
                    _guardActivator = new UnabledGuardActivator();
                    break;
            }

            switch (configuration.GuardMode)
            {
                case EGuardMode.Ratio:
                    _damageReducer = new RatioDamageReducer(configuration, guardProvider);
                    break;

                case EGuardMode.Cumulative:
                    _damageReducer = new CumulativeDamageReducer(configuration, guardProvider);
                    break;

                case EGuardMode.Base:
                default:
                    _damageReducer = new BaseDamageReducer(configuration);
                    break;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="buildableInstanceId"></param>
        /// <param name="position"></param>
        /// <param name="playerId"> Id of the player who owns the buildable </param>
        /// <param name="groupId"> Id of the group that owns the builable </param>
        /// <returns></returns>
        public ushort ReduceDamage(ushort damage, ushort assetId, uint buildableInstanceId, CSteamID playerId, CSteamID groupId)
        {
            if (!_guardActivator.TryActivateGuard(playerId, groupId))
                return damage;

            float newDamage = _damageReducer.ReduceDamage(damage, assetId, buildableInstanceId);

            // Apply max override
            if (_guardOverrides.TryGetValue(assetId, out var shieldOverride))
            {
                float minDamage = damage * (1 - shieldOverride.MaxShield);

                if (newDamage < minDamage)
                    damage = (ushort)minDamage;
                else
                    damage = (ushort)newDamage;
            }
            else
            {
                damage = (ushort)newDamage;
            }

            if (Random.value < newDamage % 1)
                damage++;

            return damage;
        }
    }
}