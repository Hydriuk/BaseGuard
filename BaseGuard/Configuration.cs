using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using System.Collections.Generic;

namespace BaseGuard
{
    public class Configuration : IPluginConfiguration
    {
        public EActivationMode ActivationMode { get; set; }

        public EGuardMode GuardMode { get; set; }

        public float BaseShield { get; set; }

        public int ActiveRaidTimer { get; set; }

        public int HistoryHoldTime { get; set; }

        public EGroupType ProtectedGroups { get; set; }

        public int DamageWarnCooldown { get; set; }

        public string ChatIcon { get; set; } = string.Empty;

        public List<GuardAsset> Guards { get; set; } = new List<GuardAsset>();

        public List<ShieldOverride> Overrides { get; set; } = new List<ShieldOverride>();
    }
}