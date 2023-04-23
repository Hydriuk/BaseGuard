using BaseGuard;
using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RocketMod
{
    public class RocketConfiguration : Configuration, IConfigurationAdapter<Configuration>, IRocketPluginConfiguration
    {
        [XmlIgnore]
        public Configuration Configuration { get => this; }

        public void LoadDefaults()
        {
            ActivationMode = EActivationMode.Offline;
            GuardMode = EGuardMode.Base;
            BaseShield = 0.5f;
            ProtectedGroups = EGroupType.Any;
            Schedule = new List<ScheduledProtection>() {};
            AllowSelfDamage = false;
            Guards = new List<GuardAsset>()
            {
                new GuardAsset()
                {
                    Id = 458,
                    Range = 16f,
                    Shield = 0.5f
                },
                new GuardAsset()
                {
                    Id = 1230,
                    Range = 64f,
                    Shield = 1f
                }
            };
            Overrides = new List<ShieldOverride>()
            {
                new ShieldOverride()
                {
                    Ids = new List<ushort>(){ 1244, 1372, 1373 },
                    BaseShield = 0f,
                    MaxShield = 0f
                }
            };

            DamageWarnCooldown = 10;
            ChatIcon = "https://i.imgur.com/V6Jc0S7.png";

            RaidDuration = 120;
            GroupHistoryDuration = 48;
            ProtectionDuration = 24;
        }
    }
}