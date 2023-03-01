using BaseGuard.API;
using BaseGuard.Events;
using BaseGuard.RocketMod.Events;
using BaseGuard.Services;
using HarmonyLib;
using Hydriuk.Unturned.RocketModModules.Adapters;
using Hydriuk.Unturned.SharedModules.Adapters;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using RocketMod;

namespace BaseGuard.RocketMod
{
    public class Plugin : RocketPlugin<RocketConfiguration>
    {
        private IActiveRaidProvider _activeRaidProvider;
        private IDamageController _damageController;
        private IGuardProvider _guardProvider;
        private IThreadAdatper _threadAdapter;
        private IConfigurationAdapter<Configuration> _configurationAdapter;
        private ITranslationsAdapter _translations;
        private IDamageWarner _damageWarner;
        private IGroupHistoryStore _groupHistoryStore;
        private IEnvironmentAdapter _environmentAdapter;
        private IProtectionDecayProvider _protectionDecayProvider;

        private BuildableDamagingEvent _buildableDamagingEvent;
        private BuildableDeployedEvent _buildableDeployedEvent;
        private BuildableDestroyedEvent _buildableDestroyedEvent;
        private PowerChangedEvent _powerChangedEvent;
        private GroupChangedEvent _groupChangedEvent;
        private PlayerConnectedEvent _playerConnectedEvent;
        private PlayerDisconnectedEvent _playerDisconnectedEvent;

        private Harmony _harmony;

        protected override void Load()
        {
            _configurationAdapter = Configuration.Instance;
            _translations = new TranslationsAdapter(Translations.Instance);
            _damageWarner = new DamageWarner(_configurationAdapter, _translations);
            _environmentAdapter = new EnvironmentAdapter(this);
            _threadAdapter = new ThreadAdapter();
            _protectionDecayProvider = new ProtectionDecayProvider(_environmentAdapter, _configurationAdapter);
            _groupHistoryStore = new GroupHistoryStore(_configurationAdapter, _environmentAdapter, _threadAdapter);
            _activeRaidProvider = new ActiveRaidProvider(_configurationAdapter, _groupHistoryStore, _protectionDecayProvider);
            _guardProvider = new GuardProvider(_configurationAdapter, _threadAdapter);
            _damageController = new DamageController(_configurationAdapter, _activeRaidProvider, _guardProvider, _damageWarner);
            _buildableDamagingEvent = new BuildableDamagingEvent(_damageController);
            _buildableDeployedEvent = new BuildableDeployedEvent(_guardProvider);
            _buildableDestroyedEvent = new BuildableDestroyedEvent(_guardProvider);
            _powerChangedEvent = new PowerChangedEvent(_guardProvider);
            _groupChangedEvent = new GroupChangedEvent(_groupHistoryStore, _protectionDecayProvider);
            _playerConnectedEvent = new PlayerConnectedEvent(_protectionDecayProvider, _groupHistoryStore);
            _playerDisconnectedEvent = new PlayerDisconnectedEvent(_protectionDecayProvider, _groupHistoryStore);

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
            _playerConnectedEvent.Dispose();
            _playerDisconnectedEvent.Dispose();
            _protectionDecayProvider.Dispose();

            _harmony.UnpatchAll("BaseGuard");
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "DamageCanceled", "This structure is under protection, you cannot damage it." },
            { "DamageReduced", "This structure is under protection. Damage dealt on this structure is reduced by {Percentage}%." }
        };
    }
}