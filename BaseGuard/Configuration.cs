using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using System.Collections.Generic;

namespace BaseGuard
{
    public class Configuration : IPluginConfiguration
    {
        public EActivationMode ActivationMode { get; set; }
        public float BaseShield { get; set; }
        public EGroupType ProtectedGroups { get; set; }
        public List<ShieldOverride> Overrides { get; set; } = new List<ShieldOverride>();

        public EGuardMode GuardMode { get; set; }
        public List<GuardAsset> Guards { get; set; } = new List<GuardAsset>();

        public int DamageWarnCooldown { get; set; }
        public string ChatIcon { get; set; } = string.Empty;

        public int GroupHistoryDuration { get; set; }
        public int RaidDuration { get; set; }
        public int ProtectionDuration { get; set; }
    }
}