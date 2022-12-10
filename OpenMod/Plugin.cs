using BaseGuard.API;
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
        public Plugin(IServiceProvider serviceProvider, IConfigurationProvider configuration, IGuardProvider guardProvider) : base(serviceProvider)
        {
        }
    }
}
