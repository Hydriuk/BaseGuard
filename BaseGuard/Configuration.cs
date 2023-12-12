using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaseGuard
{
    public class Configuration : IPluginConfiguration
    {
        public EActivationMode ActivationMode { get; set; }
        public float BaseShield { get; set; }
        public EGroupType ProtectedGroups { get; set; }
        public List<ScheduledProtection> Schedule { get; set; } = new List<ScheduledProtection>();
        public List<ShieldOverride> Overrides { get; set; } = new List<ShieldOverride>();
        public bool AllowSelfDamage { get; set; }

        public EGuardMode GuardMode { get; set; }
        public List<GuardAsset> Guards { get; set; } = new List<GuardAsset>();

        public ChatMessages ChatMessages { get; set; } = new ChatMessages();

        public double GroupHistoryDuration { get; set; }
        public double RaidDuration { get; set; }
        public double ProtectionDuration { get; set; }
    }

    public class ChatMessages
    {
        [XmlAttribute]
        public int Cooldown { get; set; }

        [XmlAttribute]
        public string ChatIcon { get; set; } = string.Empty;

        [XmlAttribute]
        public ushort EffectID { get; set; }

        [XmlAttribute]
        public string EffectTextName { get; set; } = string.Empty;

        [XmlAttribute]
        public int EffectDuration { get; set; }
    }
}