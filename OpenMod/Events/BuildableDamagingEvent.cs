using BaseGuard.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Building;
using OpenMod.Unturned.Building.Events;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BaseGuard.OpenMod.Events
{
    public class BuildableDamagingEvent : IEventListener<UnturnedBuildableDamagingEvent>
    {
        private readonly IDamageController _damageController;
        public BuildableDamagingEvent(IDamageController damageController)
        {
            _damageController = damageController;
        }

        public Task HandleEventAsync(object? sender, UnturnedBuildableDamagingEvent @event)
        {
            if (!@event.Buildable.Ownership.HasOwner ||
                @event.Buildable.Ownership.OwnerPlayerId == null ||
                @event.Buildable.Ownership.OwnerGroupId == null ||
                @event.Buildable == null)
            {
                return Task.CompletedTask;
            }

            @event.DamageAmount = (ushort)_damageController.ReduceDamage(
                @event.DamageAmount, 
                uint.Parse(@event.Buildable.BuildableInstanceId),
                new Vector3(@event.Buildable.Transform.Position.X, @event.Buildable.Transform.Position.Y, @event.Buildable.Transform.Position.Z), 
                new CSteamID(ulong.Parse(@event.Buildable.Ownership.OwnerPlayerId)),
                new CSteamID(ulong.Parse(@event.Buildable.Ownership.OwnerGroupId))
            );

            //Console.WriteLine($"[BaseGuard.BuildableDamagingEvent/HandleEventAsync] - New damage {@event.DamageAmount}");

            return Task.CompletedTask;

        }
    }
}
