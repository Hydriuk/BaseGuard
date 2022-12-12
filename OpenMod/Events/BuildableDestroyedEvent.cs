using BaseGuard.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Building.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseGuard.OpenMod.Events
{
    public class BuildableDestroyedEvent : IEventListener<UnturnedBuildableDestroyedEvent>
    {
        private readonly IGuardProvider _guardProvider;

        public BuildableDestroyedEvent(IGuardProvider guardProvider)
        {
            _guardProvider = guardProvider;
        }

        public Task HandleEventAsync(object? sender, UnturnedBuildableDestroyedEvent @event)
        {
            uint instanceId = uint.Parse(@event.Buildable.BuildableInstanceId);

            _guardProvider.RemoveBuilable(instanceId);

            return Task.CompletedTask;
        }
    }
}
