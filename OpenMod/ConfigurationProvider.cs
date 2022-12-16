using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;

namespace BaseGuard.OpenMod
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class ConfigurationProvider : Configuration
    {
        public ConfigurationProvider(IConfiguration configurator)
        {
            configurator.Bind(this);
        }
    }
}
