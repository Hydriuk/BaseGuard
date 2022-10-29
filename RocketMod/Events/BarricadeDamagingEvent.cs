using BaseGuard.API;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BaseGuard.RocketMod.Events
{
    public class BarricadeDamagingEvent : IDisposable
    {
        private readonly IDamageController _damageController;

        public BarricadeDamagingEvent(IDamageController damageController)
        {
            _damageController = damageController;

            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequested;
        }

        public void Dispose()
        {
            BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricadeRequested;
        }

        private void OnDamageBarricadeRequested(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
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

            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);
            BarricadeData data = drop.GetServersideData();
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
