using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;

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

            return true;
        }
    }
}
