using BaseGuard.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;

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

        List<GuardAsset> Guards { get; }
        List<ShieldOverride> Overrides { get; }
    }
}
