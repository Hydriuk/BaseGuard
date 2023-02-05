using BaseGuard.API;
using BaseGuard.Models;
using Rocket.API;
using System.Collections.Generic;

namespace BaseGuard.RocketMod
{
    public class ConfigurationProvider : Configuration, IConfigurationProvider, IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            ActivationMode = EActivationMode.Offline;
            GuardMode = EGuardMode.Base;
            BaseShield = 0.5f;
            ActiveRaidTimer = 120;
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