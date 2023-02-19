using BaseGuard.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IConfigurationProvider
    {
        EActivationMode ActivationMode { get; }
        EGuardMode GuardMode { get; }

        float BaseShield { get; }
        int ActiveRaidTimer { get; }
        int HistoryHoldTime { get; }
        EGroupType ProtectedGroups { get; }

        int DamageWarnCooldown { get; }
        string ChatIcon { get; } 

        List<GuardAsset> Guards { get; }
        List<ShieldOverride> Overrides { get; }
    }
}
