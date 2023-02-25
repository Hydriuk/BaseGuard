using Hydriuk.Unturned.SharedModules.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;

namespace BaseGuard.OpenMod
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class ConfigurationProvider : Configuration, IConfigurationAdapter<Configuration>
    {
        public Configuration Configuration { get; private set; }

        public ConfigurationProvider(IConfiguration configurator)
        {

            Configuration = new Configuration();
            configurator.Bind(Configuration);
        }
    }
}