using BaseGuard.API;
using BaseGuard.Events;
using BaseGuard.RocketMod.Events;
using BaseGuard.Services;
using HarmonyLib;
using Rocket.Core.Plugins;
using ThreadModule.API;
using ThreadModule.RocketMod;

namespace BaseGuard.RocketMod
{
    public class Plugin : RocketPlugin<ConfigurationProvider>
    {
        private IActiveRaidProvider _activeRaidProvider;
        private IDamageController _damageController;
        private IGuardProvider _guardProvider;
        private IThreadAdatper _threadAdapter;
        private IConfigurationProvider _configurationProvider;

        private BuildableDamagingEvent _buildableDamagingEvent;
        private BuildableDeployedEvent _buildableDeployedEvent;
        private BuildableDestroyedEvent _buildableDestroyedEvent;
        private PowerChangedEvent _powerChangedEvent;

        private Harmony _harmony;

        protected override void Load()
        {
            _configurationProvider = Configuration.Instance;
            _activeRaidProvider = new ActiveRaidProvider(_configurationProvider);
            _threadAdapter = new ThreadAdapter();
            _guardProvider = new GuardProvider(_configurationProvider, _threadAdapter);
            _damageController = new DamageController(_configurationProvider, _activeRaidProvider, _guardProvider);

            _buildableDamagingEvent = new BuildableDamagingEvent(_damageController);
            _buildableDeployedEvent = new BuildableDeployedEvent(_guardProvider);
            _buildableDestroyedEvent = new BuildableDestroyedEvent(_guardProvider);
            _powerChangedEvent = new PowerChangedEvent(_guardProvider);

            _harmony = new Harmony("BaseGuard");
            _harmony.PatchAll();
        }

        protected override void Unload()
        {
            _buildableDamagingEvent.Dispose();
            _buildableDeployedEvent.Dispose();
            _buildableDestroyedEvent.Dispose();
            _powerChangedEvent.Dispose();

            _harmony.UnpatchAll("BaseGuard");
        }
    }
}