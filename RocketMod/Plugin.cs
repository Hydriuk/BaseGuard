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
        public static Plugin Instance;

        public IActiveRaidProvider ActiveRaidProvider;
        public IDamageController DamageController;
        public IGuardProvider GuardProvider;
        public IThreadAdatper ThreadAdapter;
        public IConfigurationAdapter<Configuration> ConfigurationAdapter;
        public ITranslationsAdapter TranslationsAdapter;
        public IDamageWarner DamageWarner;
        public IGroupHistoryStore GroupHistoryStore;
        public IEnvironmentAdapter EnvironmentAdapter;
        public IProtectionDecayProvider ProtectionDecayProvider;
        public IProtectionScheduler ProtectionScheduler;

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
            Instance = this;

            ConfigurationAdapter = Configuration.Instance;
            ThreadAdapter = new ThreadAdapter();
            TranslationsAdapter = new TranslationsAdapter(base.Translations.Instance);
            DamageWarner = new DamageWarner(ConfigurationAdapter, TranslationsAdapter, ThreadAdapter);
            EnvironmentAdapter = new EnvironmentAdapter(this);
            ProtectionScheduler = new ProtectionScheduler(ConfigurationAdapter, TranslationsAdapter, ThreadAdapter);
            ProtectionDecayProvider = new ProtectionDecayProvider(EnvironmentAdapter, ConfigurationAdapter);
            GroupHistoryStore = new GroupHistoryStore(ConfigurationAdapter, EnvironmentAdapter, ThreadAdapter);
            ActiveRaidProvider = new ActiveRaidProvider(ConfigurationAdapter, GroupHistoryStore, ProtectionDecayProvider);
            GuardProvider = new GuardProvider(ConfigurationAdapter, ThreadAdapter);
            DamageController = new DamageController(ConfigurationAdapter, ActiveRaidProvider, GuardProvider, ProtectionScheduler, DamageWarner);
            _buildableDamagingEvent = new BuildableDamagingEvent(DamageController);
            _buildableDeployedEvent = new BuildableDeployedEvent(GuardProvider);
            _buildableDestroyedEvent = new BuildableDestroyedEvent(GuardProvider);
            _powerChangedEvent = new PowerChangedEvent(GuardProvider);
            _groupChangedEvent = new GroupChangedEvent(GroupHistoryStore, ProtectionDecayProvider);
            _playerConnectedEvent = new PlayerConnectedEvent(ProtectionDecayProvider, GroupHistoryStore);
            _playerDisconnectedEvent = new PlayerDisconnectedEvent(ProtectionDecayProvider, GroupHistoryStore);

            _harmony = new Harmony("BaseGuard");
            _harmony.PatchAll();
        }

        protected override void Unload()
        {
            GroupHistoryStore.Dispose();
            _buildableDamagingEvent.Dispose();
            _buildableDeployedEvent.Dispose();
            _buildableDestroyedEvent.Dispose();
            _powerChangedEvent.Dispose();
            _groupChangedEvent.Dispose();
            _playerConnectedEvent.Dispose();
            _playerDisconnectedEvent.Dispose();
            ProtectionDecayProvider.Dispose();
            ProtectionScheduler.Dispose();

            _harmony.UnpatchAll("BaseGuard");
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "DamageCanceled", "This structure is under protection, you cannot damage it." },
            { "DamageReduced", "This structure is under protection. Damage dealt on this structure is reduced by {Percentage}%." },
            { "ProtectionActivated", "Protection is now active ! Protection will be deactivated in {TotalHours}h {Minutes}min" },
            { "ProtectionDeactivated", "Protection is now deactivated ! Protection will be activated in {TotalHours}h {Minutes}min" },
            { "NoProtectionSchedule", "This server does not use protection schedules" }
        };
    }
}