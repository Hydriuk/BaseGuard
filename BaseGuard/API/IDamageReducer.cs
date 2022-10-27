using BaseGuard.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IDamageReducer
    {
        float ReduceDamage(float damage, List<Guard> guards);
    }
}
