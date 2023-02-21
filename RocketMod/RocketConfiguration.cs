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
            ActiveRaidTimer = 120;
            HistoryHoldTime = 108000;
            ProtectedGroups = EGroupType.All;
            DamageWarnCooldown = 10;
            ChatIcon = "https://i.imgur.com/V6Jc0S7.png";
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
                    Id = 1373,
                    BaseShield = 0f,
                    MaxShield = 0f
                }
            };
        }
    }
}