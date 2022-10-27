using BaseGuard.API;
using BaseGuard.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Text;
using UnityEngine;

namespace BaseGuard.Services.DamageReducers
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class BaseDamageReducer : IDamageReducer
    {
        private readonly float _baseShield;

        public BaseDamageReducer(IConfigurationProvider configuration)
        {
            _baseShield = configuration.BaseShield;
        }

        public virtual float ReduceDamage(float damage, uint buildableInstanceId, Vector3 position) => damage * (1 - _baseShield);
    }
}
