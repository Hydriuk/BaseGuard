using BaseGuard.API;
using HarmonyLib;
using JetBrains.Annotations;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseGuard.RocketMod.Events
{
    public class BuildableDestroyedEvent : IDisposable
    {
        private readonly IGuardProvider _guardProvider;

        public BuildableDestroyedEvent(IGuardProvider guardProvider)
        {
            _guardProvider = guardProvider;

            DestroyedPatches.BuildableDestroyed += OnBuildableDestroyed;
        }

        public void Dispose()
        {
            DestroyedPatches.BuildableDestroyed -= OnBuildableDestroyed;
        }

        public void OnBuildableDestroyed(uint instanceId)
        {
            Console.WriteLine("remove build");
            _guardProvider.RemoveBuilable(instanceId);
        }
    }

    [HarmonyPatch]
    public class DestroyedPatches
    {
        public delegate void BuildableDestroyedHandler(uint instanceId);

        public static event BuildableDestroyedHandler BuildableDestroyed;

        [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveDestroyBarricade))]
        [HarmonyPrefix]
        private static void BarricadeDestroyed(NetId netId)
        {
            var barricade = NetIdRegistry.Get<BarricadeDrop>(netId);

            if (barricade == null)
                return;

            BuildableDestroyed?.Invoke(barricade.instanceID);
        }

        [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.ReceiveDestroyStructure))]
        [HarmonyPrefix]
        private static void StructureDestroyed(NetId netId)
        {
            var structure = NetIdRegistry.Get<StructureDrop>(netId);

            if (structure == null)
                return;

            BuildableDestroyed?.Invoke(structure.instanceID);
        }
    }
}
