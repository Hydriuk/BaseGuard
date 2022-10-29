using BaseGuard.API;
using BaseGuard.RocketMod.Events;
using BaseGuard.Services;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BaseGuard.RocketMod
{
    public class Plugin : RocketPlugin<ConfigurationProvider>
    {
        public Plugin Instance { get; }

        private IActiveRaidProvider _activeRaidProvider;
        private IDamageController _damageController;
        private IGuardProvider _guardProvider;
        private IConfigurationProvider _configurationProvider;

        private BarricadeDamagingEvent _barricadeDamagingEvent;
        private StructureDamagingEvent _structureDamagingEvent;

        public Plugin()
        {
            Instance = this;
        }

        protected override void Load()
        {
            _configurationProvider = Configuration.Instance;
            _activeRaidProvider = new ActiveRaidProvider();
            _guardProvider = new GuardProvider(_configurationProvider);
            _damageController = new DamageController(_configurationProvider, _activeRaidProvider, _guardProvider);

            _barricadeDamagingEvent = new BarricadeDamagingEvent(_damageController);
            _structureDamagingEvent = new StructureDamagingEvent(_damageController);
        }

        protected override void Unload()
        {
            _barricadeDamagingEvent.Dispose();
            _structureDamagingEvent.Dispose();
        }
    }
}
