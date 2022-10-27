using BaseGuard.API;
using BaseGuard.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard
{
    public class Configuration : IConfigurationProvider
    {
        public EActivationMode ActivationMode { get; set; }

        public EGuardMode GuardMode { get; set; }

        public float BaseShield { get; set; }

        public int ActiveRaidTimer { get; set; }

        public List<GuardAsset> Guards { get; set; } = new List<GuardAsset>();

        public List<ShieldOverwrite> Overwrites { get; set; } = new List<ShieldOverwrite>();
    }
}
