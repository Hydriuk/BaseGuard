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

namespace BaseGuard.Services.DamageReducers
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class CumulativeDamageReducer : IDamageReducer
    {
        public float ReduceDamage(float damage, List<Guard> guards)
        {
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
