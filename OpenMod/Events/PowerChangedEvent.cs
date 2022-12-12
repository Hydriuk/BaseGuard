using BaseGuard.API;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.Core.Eventing;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BaseGuard.OpenMod.Events
{
    public delegate void PowerUpdatedHandler(uint instanceId, bool powered);

    public class PowerChangedEvent : IDisposable
    {
        private readonly IGuardProvider _guardProvider;

        public PowerChangedEvent(IGuardProvider guardProvider)
        {
            _guardProvider = guardProvider;

            PowerPatches.PowerUpdated += OnPowerUpdated;
        }   

        public void Dispose()
        {
            PowerPatches.PowerUpdated -= OnPowerUpdated;
        }

        private void OnPowerUpdated(uint instanceId, bool powered)
        {
            _guardProvider.UpdateGuard(instanceId, powered);
        }
    }

    [HarmonyPatch]
    public class PowerPatches
    {
        public static event PowerUpdatedHandler? PowerUpdated;

        // GENERATOR
        [HarmonyPatch(typeof(InteractableGenerator), nameof(InteractableGenerator.updatePowered))]
        [HarmonyPostfix]
        private static void GeneratorPoweredPostfix(InteractableGenerator __instance)
        {
            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(__instance.transform);

            PowerUpdated?.Invoke(drop.instanceID, __instance.isPowered);
        }

        // SAFEZONE
        [HarmonyPatch(typeof(InteractableSafezone), nameof(InteractableSafezone.updatePowered))]
        [HarmonyPostfix]
        private static void SafezonePoweredPostfix(InteractableSafezone __instance)
        {
            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(__instance.transform);

            PowerUpdated?.Invoke(drop.instanceID, __instance.isPowered && __instance.isWired);
        }

        [HarmonyPatch(typeof(InteractableSafezone), nameof(InteractableSafezone.updateWired))]
        [HarmonyPostfix]
        private static void SafezoneWiredPostfix(InteractableSafezone __instance)
        {
            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(__instance.transform);

            PowerUpdated?.Invoke(drop.instanceID, __instance.isPowered && __instance.isWired);
        }
        
        // OXYGENATOR
        [HarmonyPatch(typeof(InteractableOxygenator), nameof(InteractableOxygenator.updatePowered))]
        [HarmonyPostfix]
        private static void OxygenatorPoweredPostfix(InteractableOxygenator __instance)
        {
            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(__instance.transform);

            PowerUpdated?.Invoke(drop.instanceID, __instance.isPowered && __instance.isWired);
        }

        [HarmonyPatch(typeof(InteractableOxygenator), nameof(InteractableOxygenator.updateWired))]
        [HarmonyPostfix]
        private static void OxygenatorWiredPostfix(InteractableOxygenator __instance)
        {
            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(__instance.transform);

            PowerUpdated?.Invoke(drop.instanceID, __instance.isPowered && __instance.isWired);
        }
    }
}
