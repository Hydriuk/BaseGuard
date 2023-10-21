using BaseGuard.API;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;

namespace BaseGuard.RocketMod.Events
{
    public class BuildableDamagingEvent : IDisposable
    {
        private readonly IDamageController _damageController;

        public BuildableDamagingEvent(IDamageController damageController)
        {
            _damageController = damageController;

            StructureManager.onDamageStructureRequested += OnDamageStructureRequested;
            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequested;
        }

        public void Dispose()
        {
            StructureManager.onDamageStructureRequested -= OnDamageStructureRequested;
            BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricadeRequested;
        }

        private void OnDamageStructureRequested(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            bool isTrapDamage = damageOrigin == EDamageOrigin.Trap_Explosion;

            if (!isTrapDamage && instigatorSteamID == CSteamID.Nil)
                return;

            if (isTrapDamage)
            {
                ItemAsset asset = (ItemAsset)Assets.find(EAssetType.ITEM, ushort.Parse(structureTransform.name));
                if (asset.type == EItemType.TRAP)
                    return;
            }

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
                drop.asset.id,
                data.instanceID,
                new CSteamID(data.owner),
                new CSteamID(data.group),
                instigatorSteamID
            );
        }

        private void OnDamageBarricadeRequested(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            bool isTrapDamage = damageOrigin == EDamageOrigin.Trap_Explosion;

            if (!isTrapDamage && instigatorSteamID == CSteamID.Nil)
                return;

            if(isTrapDamage)
            {
                ItemAsset asset = (ItemAsset)Assets.find(EAssetType.ITEM, ushort.Parse(barricadeTransform.name));
                if (asset.type == EItemType.TRAP)
                    return;
            }

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
                drop.asset.id,
                data.instanceID,
                new CSteamID(data.owner),
                new CSteamID(data.group),
                instigatorSteamID
            );
        }
    }
}