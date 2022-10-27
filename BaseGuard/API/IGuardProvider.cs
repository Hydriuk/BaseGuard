using BaseGuard.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IGuardProvider
    {
        List<Guard> GetGuards(uint buildableInstanceId, Vector3 position);
    }
}
