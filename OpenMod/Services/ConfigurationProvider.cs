using BaseGuard.API;
using BaseGuard.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IConfigurationProvider = BaseGuard.API.IConfigurationProvider;

namespace BaseGuard.OpenMod.Services
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
