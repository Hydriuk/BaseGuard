using BaseGuard.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Building.Events;
using SDG.Unturned;
using Steamworks;
using System.Threading.Tasks;

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
            // Cancel damage reduction if buildable has no owner
            if (
                !@event.Buildable.Ownership.HasOwner ||
                @event.Buildable.Ownership.OwnerPlayerId == null && @event.Buildable.Ownership.OwnerGroupId == null ||
                @event.Buildable == null
            )
            {
                return Task.CompletedTask;
            }

            // Cancel damage reduction for technical gameplay damages
            if (@event is
                {
                    DamageOrigin:
                        EDamageOrigin.Carepackage_Timeout or
                        EDamageOrigin.Charge_Self_Destruct or
                        EDamageOrigin.Horde_Beacon_Self_Destruct or
                        EDamageOrigin.Plant_Harvested or
                        EDamageOrigin.VehicleDecay
                }
            )
            {
                return Task.CompletedTask;
            }

            @event.DamageAmount = _damageController.ReduceDamage(
                @event.DamageAmount,
                ushort.Parse(@event.Buildable.Asset.BuildableAssetId),
                uint.Parse(@event.Buildable.BuildableInstanceId),
                new CSteamID(ulong.Parse(@event.Buildable.Ownership.OwnerPlayerId)),
                new CSteamID(ulong.Parse(@event.Buildable.Ownership.OwnerGroupId)),
                @event.Instigator?.SteamId ?? CSteamID.Nil
            );

            return Task.CompletedTask;
        }
    }
}