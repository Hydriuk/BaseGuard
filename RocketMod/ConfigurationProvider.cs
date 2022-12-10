using BaseGuard.API;
using BaseGuard.Models;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Guards = new List<GuardAsset>();
            Overwrites = new List<ShieldOverwrite>();
        }
    }
}
