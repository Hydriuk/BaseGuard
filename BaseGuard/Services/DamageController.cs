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
using System.IdentityModel.Protocols.WSTrust;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class DamageController : IDamageController
    {
        private readonly IGuardActivator _guardActivator;
        private readonly IDamageReducer _damageReducer;
        private readonly IGuardProvider _guardProvider;

        public DamageController(IConfigurationProvider configuration, IActiveRaidProvider activeRaidProvider, IGuardProvider guardProvider)
        {
            _guardProvider = guardProvider;

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
                case EGuardMode.All:
                    _damageReducer = new AllDamageReducer();
                    break;

                case EGuardMode.Ratio:
                    _damageReducer = new RatioDamageReducer();
                    break;

                case EGuardMode.Cumulative:
                default:
                    _damageReducer = new CumulativeDamageReducer();
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
        /// <exception cref="NotImplementedException"></exception>
        public float ReduceDamage(float damage, uint buildableInstanceId, Vector3 position, CSteamID playerId, CSteamID groupId)
        {
            if (!_guardActivator.TryActivateGuard(playerId, groupId))
                return damage;

            List<Guard> guards = _guardProvider.GetGuards(buildableInstanceId, position);

            damage = _damageReducer.ReduceDamage(damage, guards);

            return damage;
        }
    }
}
