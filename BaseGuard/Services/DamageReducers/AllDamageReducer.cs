using BaseGuard.API;
using BaseGuard.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.Services.DamageReducers
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class AllDamageReducer : IDamageReducer
    {
        public float ReduceDamage(float damage, List<Guard> guards)
        {
            throw new NotImplementedException();
        }
    }
}
