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
        public Plugin(IServiceProvider serviceProvider, IConfigurationProvider configuration) : base(serviceProvider)
        {
            Console.WriteLine(configuration.ActiveRaidTimer);
            Console.WriteLine(configuration.ActivationMode);
            Console.WriteLine(configuration.BaseShield);
            Console.WriteLine(configuration.GuardMode);
            Console.WriteLine(configuration.Guards.Count);
            Console.WriteLine(configuration.Overwrites.Count);
        }

    }
}
