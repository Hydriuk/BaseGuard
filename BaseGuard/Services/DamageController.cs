using BaseGuard.API;
using BaseGuard.Models;
using BaseGuard.Services.DamageReducers;
using BaseGuard.Services.GuardActivators;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public DamageController(IConfigurationProvider configuration, IActiveRaidProvider activeRaidProvider, IGuardProvider guardProvider)
        {
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
        public ushort ReduceDamage(ushort damage, uint buildableInstanceId, Vector3 position, CSteamID playerId, CSteamID groupId)
        {
            if (!_guardActivator.TryActivateGuard(playerId, groupId))
                return damage;

            float newDamage = _damageReducer.ReduceDamage(damage, buildableInstanceId, position);

            damage = (ushort)newDamage;

            if (Random.value < newDamage % 1)
                damage++;

            return damage;
        }
    }
}
