using BaseGuard.API;
using BaseGuard.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaseGuard.Services.DamageReducers
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class RatioDamageReducer : BaseDamageReducer, IDamageReducer
    {
        public RatioDamageReducer(IConfigurationProvider configuration) : base(configuration)
        {
        }

        public float ReduceDamage(float damage, uint buildableInstanceId, Vector3 position)
        {
            throw new NotImplementedException();
        }
    }
}
