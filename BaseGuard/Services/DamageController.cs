using BaseGuard.API;
using BaseGuard.Models;
using BaseGuard.Services.DamageReducers;
using BaseGuard.Services.GuardActivators;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IDamageWarner _damageWarner;
        private readonly Dictionary<ushort, ShieldOverride> _guardOverrides;

        public DamageController(
            IConfigurationProvider configuration, 
            IActiveRaidProvider activeRaidProvider, 
            IGuardProvider guardProvider, 
            IDamageWarner damageWarner)
        {
            _damageWarner = damageWarner;

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
        public ushort ReduceDamage(ushort damage, ushort assetId, uint buildableInstanceId, CSteamID playerId, CSteamID groupId, CSteamID instigatorId)
        {
            float newDamage = damage;

            if(_guardActivator.TryActivateGuard(playerId, groupId))
                newDamage = _damageReducer.ReduceDamage(damage, assetId, buildableInstanceId);

            // Apply max override
            newDamage = ApplyOverride(damage, newDamage, assetId);

            if (instigatorId != CSteamID.Nil)
                _damageWarner.TryWarn(PlayerTool.getPlayer(instigatorId), damage, newDamage);

            damage = ConvertDamage(newDamage);

            return damage;
        }

        private float ApplyOverride(float baseDamage, float currentDamage, ushort assetId)
        {
            if (!_guardOverrides.TryGetValue(assetId, out var shieldOverride))
                return currentDamage;

            // float minDamage = baseDamage * (1 - shieldOverride.MinShield);

            float maxDamage = baseDamage * (1 - shieldOverride.MaxShield);

            //if (currentDamage < minDamage)
            //    return minShield

            //if (currentDamage > maxDamage)
            //    return maxDamage;

            return currentDamage > maxDamage ? maxDamage : currentDamage;
        }

        private ushort ConvertDamage(float damageToConvert)
        {
            ushort damage = (ushort)damageToConvert;

            if (Random.value < damageToConvert % 1)
                damage++;

            return damage;
        }
    }
}