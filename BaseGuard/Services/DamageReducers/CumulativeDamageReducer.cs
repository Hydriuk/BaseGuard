using BaseGuard.API;
using BaseGuard.Models;
using Cysharp.Threading.Tasks;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace BaseGuard.Services.DamageReducers
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class CumulativeDamageReducer : BaseDamageReducer, IDamageReducer
    {
        private readonly IGuardProvider _guardProvider;

        public CumulativeDamageReducer(IConfigurationProvider configuration, IGuardProvider guardProvider) : base(configuration)
        {
            _guardProvider = guardProvider;
        }

        public override float ReduceDamage(float damage, uint buildableInstanceId, Vector3 position)
        {
            damage = base.ReduceDamage(damage, buildableInstanceId, position);

            List<Guard> guards = _guardProvider.GetGuards(buildableInstanceId, position);

            if (guards.Count == 0)
                return damage;

            float protectionCoefficient = guards
                .Select(guard => guard.Shield)
                .Aggregate((totalShield, guardShield) => totalShield += guardShield);

            protectionCoefficient = Mathf.Clamp01(protectionCoefficient);

            return damage * (1 - protectionCoefficient);
        }
    }
}
