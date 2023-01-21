using BaseGuard.API;
using BaseGuard.Extensions;
using SDG.Unturned;
using System;

namespace BaseGuard.RocketMod.Events
{
    public class BuildableDeployedEvent : IDisposable
    {
        private readonly IGuardProvider _guardProvider;

        public BuildableDeployedEvent(IGuardProvider guardProvider)
        {
            _guardProvider = guardProvider;

            BarricadeManager.onBarricadeSpawned += OnBarricadeDeployed;
            StructureManager.onStructureSpawned += OnStructureDeployed;
        }

        public void Dispose()
        {
            BarricadeManager.onBarricadeSpawned -= OnBarricadeDeployed;
            StructureManager.onStructureSpawned -= OnStructureDeployed;
        }

        private void OnBarricadeDeployed(BarricadeRegion region, BarricadeDrop drop)
        {
            _guardProvider.AddBuilable(drop.instanceID, drop.model.position);

            bool isActive = drop.interactable.IsActive();

            _guardProvider.AddGuard(drop.asset.id, drop.instanceID, drop.model.position, isActive);
        }

        private void OnStructureDeployed(StructureRegion region, StructureDrop drop)
        {
            _guardProvider.AddBuilable(drop.instanceID, drop.model.position);

            _guardProvider.AddGuard(drop.asset.id, drop.instanceID, drop.model.position, true);
        }
    }
}