using BaseGuard.API;
using BaseGuard.OpenMod.Events;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("BaseGuard", DisplayName = "BaseGuard", Author = "Hydriuk")]

namespace BaseGuard.OpenMod
{
    public class Plugin : OpenModUnturnedPlugin
    {
        private readonly PowerChangedEvent _powerChangedEvent;

        private readonly IServiceProvider _serviceProvider;

        public Plugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _powerChangedEvent = new PowerChangedEvent((IGuardProvider)serviceProvider.GetService(typeof(IGuardProvider)));
        }

        protected override UniTask OnLoadAsync()
        {
            _serviceProvider.GetRequiredService<IGroupHistoryStore>();

            return UniTask.CompletedTask;
        }

        protected override UniTask OnUnloadAsync()
        {
            _powerChangedEvent.Dispose();

            return UniTask.CompletedTask;
        }
    }
}