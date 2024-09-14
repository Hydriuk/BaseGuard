using SDG.Unturned;

namespace BaseGuard.Extensions
{
    public static class InteractableExtension
    {
        public static bool IsActive(this Interactable interactable)
        {
            if (interactable == null)
                return true;

            if (interactable is InteractableGenerator generator)
                return generator.isPowered && generator.fuel > 0;

            if (interactable is InteractableSafezone safezone)
                return safezone.isPowered && safezone.isWired;

            if (interactable is InteractableOxygenator oxygenator)
                return oxygenator.isPowered && oxygenator.isWired;

            if (interactable is InteractableTank tank)
                return tank.amount > 0;

            return true;
        }
    }
}