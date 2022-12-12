using BaseGuard.API;
using BaseGuard.OpenMod.Events;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;


[assembly: PluginMetadata("BaseGuard", DisplayName = "BaseGuard", Author = "Hydriuk")]

namespace BaseGuard.OpenMod
{
    public class Plugin : OpenModUnturnedPlugin
    {
        private readonly PowerChangedEvent _powerChangedEvent;

        public Plugin(IServiceProvider serviceProvider, IConfigurationProvider configuration, IGuardProvider guardProvider) : base(serviceProvider)
        {
            _powerChangedEvent = new PowerChangedEvent(guardProvider);
        }

        protected override UniTask OnUnloadAsync()
        {
            _powerChangedEvent.Dispose();

            return UniTask.CompletedTask;
        }
    }
}
