using BaseGuard.API;
using Hydriuk.Unturned.SharedModules.Adapters;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class DamageWarner : IDamageWarner
    {
        private readonly ITranslationsAdapter _translationsAdapter;

        private readonly int _coolddown;
        private readonly string _chatIcon;

        private readonly Dictionary<Player, float> _lastWarnTimeProvider = new Dictionary<Player, float>();

        public DamageWarner(IConfigurationAdapter<Configuration> confAdapter, ITranslationsAdapter translations)
        {
            _translationsAdapter = translations;

            _coolddown = confAdapter.Configuration.DamageWarnCooldown;
            _chatIcon = confAdapter.Configuration.ChatIcon;
        }

        public void TryWarn(Player player, float oldDamage, float newDamage)
        {
            if (player == null || oldDamage == newDamage)
                return;

            // Check & Update player cooldown
            if (_lastWarnTimeProvider.TryGetValue(player, out float lastWarnTime))
            {
                // Cancel if cooldown has not passed
                if (lastWarnTime + _coolddown > Time.realtimeSinceStartup)
                    return;

                _lastWarnTimeProvider[player] = Time.realtimeSinceStartup;
            }
            else
            {
                _lastWarnTimeProvider.Add(player, Time.realtimeSinceStartup);
            }

            if (newDamage == 0 && !string.IsNullOrWhiteSpace(_translationsAdapter["DamageCanceled"]))
            {
                ChatManager.serverSendMessage(
                    _translationsAdapter["DamageCanceled"],
                    Color.yellow,
                    toPlayer: player.channel.owner,
                    iconURL: _chatIcon
                );
            }
            else if (newDamage < oldDamage && !string.IsNullOrWhiteSpace(_translationsAdapter["DamageReduced"]))
            {
                var damagePercent = (1 - (newDamage / oldDamage)) * 100;

                ChatManager.serverSendMessage(
                    _translationsAdapter["DamageReduced", new { Percentage = damagePercent.ToString("F0") }],
                    Color.yellow,
                    toPlayer: player.channel.owner,
                    iconURL: _chatIcon
                );
            }
        }
    }
}