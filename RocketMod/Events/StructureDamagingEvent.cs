using BaseGuard.API;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BaseGuard.RocketMod.Events
{
    public class StructureDamagingEvent : IDisposable
    {
        private readonly IDamageController _damageController;

        public StructureDamagingEvent(IDamageController damageController)
        {
            _damageController = damageController;

            StructureManager.onDamageStructureRequested += OnDamageStructureRequested;
        }

        public void Dispose()
        {
            StructureManager.onDamageStructureRequested += OnDamageStructureRequested;
        }

        private void OnDamageStructureRequested(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (instigatorSteamID == CSteamID.Nil)
                return;

            switch (damageOrigin)
            {
                case EDamageOrigin.Carepackage_Timeout:
                case EDamageOrigin.Charge_Self_Destruct:
                case EDamageOrigin.Horde_Beacon_Self_Destruct:
                case EDamageOrigin.Plant_Harvested:
                case EDamageOrigin.VehicleDecay:
                    return;
            }

            StructureDrop drop = StructureManager.FindStructureByRootTransform(structureTransform);
            StructureData data = drop.GetServersideData();

            pendingTotalDamage = _damageController.ReduceDamage(
                pendingTotalDamage,
                data.instanceID,
                data.point,
                new CSteamID(data.owner),
                new CSteamID(data.group)
            );
        }
    }
}
