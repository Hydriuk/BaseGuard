using BaseGuard.API;
using BaseGuard.Extensions;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Building.Events;
using System.Threading.Tasks;
using UnityEngine;

namespace BaseGuard.OpenMod.Events
{
    internal class BuildableDeployedEvent : IEventListener<UnturnedBuildableDeployedEvent>
    {
        private readonly IGuardProvider _guardProvider;

        public BuildableDeployedEvent(IGuardProvider guardProvider)
        {
            _guardProvider = guardProvider;
        }

        public Task HandleEventAsync(object? sender, UnturnedBuildableDeployedEvent @event)
        {
            Vector3 position = new Vector3(
                @event.Buildable.Transform.Position.X,
                @event.Buildable.Transform.Position.Y,
                @event.Buildable.Transform.Position.Z
            );

            ushort assetId = ushort.Parse(@event.Buildable.Asset.BuildableAssetId);
            uint instanceId = uint.Parse(@event.Buildable.BuildableInstanceId);

            _guardProvider.AddBuilable(instanceId, position);

            bool isActive = true;
            if (@event is UnturnedBarricadeDeployedEvent deployedEvent)
                isActive = deployedEvent.Buildable.Interactable.IsActive();

            _guardProvider.AddGuard(assetId, instanceId, position, isActive);

            return Task.CompletedTask;
        }
    }
}