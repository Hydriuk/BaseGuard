using BaseGuard.API;
using BaseGuard.Events;
using BaseGuard.RocketMod.Events;
using BaseGuard.Services;
using EnvironmentModule.API;
using EnvironmentModule.RocketMod;
using HarmonyLib;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using ThreadModule.API;
using ThreadModule.RocketMod;
using TranslationsModule.API;
using TranslationsModule.RocketMod;

namespace BaseGuard.RocketMod
{
    public class Plugin : RocketPlugin<ConfigurationProvider>
    {
        private IActiveRaidProvider _activeRaidProvider;
        private IDamageController _damageController;
        private IGuardProvider _guardProvider;
        private IThreadAdatper _threadAdapter;
        private IConfigurationProvider _configurationProvider;
        private ITranslationsAdapter _translations;
        private IDamageWarner _damageWarner;
        private IGroupHistoryStore _groupHistoryStore;
        private IEnvironmentProvider _environmentProvider;

        private BuildableDamagingEvent _buildableDamagingEvent;
        private BuildableDeployedEvent _buildableDeployedEvent;
        private BuildableDestroyedEvent _buildableDestroyedEvent;
        private PowerChangedEvent _powerChangedEvent;
        private GroupChangedEvent _groupChangedEvent;

        private Harmony _harmony;

        protected override void Load()
        {
            _configurationProvider = Configuration.Instance;
            _translations = new TranslationsAdapter(Translations.Instance);
            _damageWarner = new DamageWarner(_configurationProvider, _translations);
            _environmentProvider = new EnvironmentProvider(this);
            _threadAdapter = new ThreadAdapter();
            _groupHistoryStore = new GroupHistoryStore(_configurationProvider, _environmentProvider, _threadAdapter);
            _activeRaidProvider = new ActiveRaidProvider(_configurationProvider, _groupHistoryStore);
            _guardProvider = new GuardProvider(_configurationProvider, _threadAdapter);
            _damageController = new DamageController(_configurationProvider, _activeRaidProvider, _guardProvider, _damageWarner);

            _buildableDamagingEvent = new BuildableDamagingEvent(_damageController);
            _buildableDeployedEvent = new BuildableDeployedEvent(_guardProvider);
            _buildableDestroyedEvent = new BuildableDestroyedEvent(_guardProvider);
            _powerChangedEvent = new PowerChangedEvent(_guardProvider);
            _groupChangedEvent = new GroupChangedEvent(_groupHistoryStore);

            _harmony = new Harmony("BaseGuard");
            _harmony.PatchAll();
        }

        protected override void Unload()
        {
            _groupHistoryStore.Dispose();

            _buildableDamagingEvent.Dispose();
            _buildableDeployedEvent.Dispose();
            _buildableDestroyedEvent.Dispose();
            _powerChangedEvent.Dispose();
            _groupChangedEvent.Dispose();

            _harmony.UnpatchAll("BaseGuard");
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "DamageCanceled", "This structure is under protection, you cannot damage it." },
            { "DamageReduced", "This structure is under protection. Damage dealt on this structure is reduced by {Percentage}%." }
        };
    }
}