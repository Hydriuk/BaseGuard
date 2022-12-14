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
        IEnumerable<Guard> GetGuards(uint buildableInstanceId, Vector3 position);
        void AddGuard(ushort assetId, uint buildableInstanceId, Vector3 position, bool isActive);
        void AddBuilable(uint instanceId, Vector3 position);
        void RemoveBuilable(uint instanceId);
        void UpdateGuard(uint buildableInstanceId, bool active);
    }
}
