using BaseGuard.API;
using Hydriuk.Unturned.SharedModules.Adapters;
using Hydriuk.Unturned.SharedModules.Extensions;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class DamageWarner : IDamageWarner
    {
        private readonly ITranslationsAdapter _translationsAdapter;
        private readonly IThreadAdatper _threadAdatper;

        private readonly int _coolddown;
        private readonly string _chatIcon;
        private readonly ushort _effectID;
        private readonly string _textName;
        private readonly int _effectDuration;
        private short _effectKey { get => (short)(_effectID - short.MaxValue); }

        private readonly Dictionary<Player, float> _lastWarnTimeProvider = new Dictionary<Player, float>();

        public DamageWarner(IConfigurationAdapter<Configuration> confAdapter, ITranslationsAdapter translations, IThreadAdatper threadAdatper)
        {
            _threadAdatper = threadAdatper;
            _translationsAdapter = translations;

            _coolddown = confAdapter.Configuration.ChatMessages.Cooldown;
            _chatIcon = confAdapter.Configuration.ChatMessages.ChatIcon;
            _effectID = confAdapter.Configuration.ChatMessages.EffectID;
            _textName = confAdapter.Configuration.ChatMessages.EffectTextName;
            _effectDuration = confAdapter.Configuration.ChatMessages.EffectDuration;
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
                _threadAdatper.RunOnMainThread(() =>
                {
                    if (_effectID != ushort.MinValue)
                    {
                        EffectManager.sendUIEffect(_effectID, _effectKey, player.GetTransportConnection(), true);
                        EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, "Notification", true);
                        EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, _textName, _translationsAdapter["DamageCanceled"]);
                        _threadAdatper.RunOnThreadPool(async () =>
                        {
                            await Task.Delay(_effectDuration * 1000);
                            _threadAdatper.RunOnMainThread(() => EffectManager.askEffectClearByID(_effectID, player.GetTransportConnection()));
                        });
                    }
                    else
                    {
                        ChatManager.serverSendMessage(
                            _translationsAdapter["DamageCanceled"],
                            Color.yellow,
                            toPlayer: player.channel.owner,
                            iconURL: _chatIcon
                        );
                    }
                });
            }
            else if (newDamage < oldDamage && !string.IsNullOrWhiteSpace(_translationsAdapter["DamageReduced"]))
            {
                var damagePercent = (1 - (newDamage / oldDamage)) * 100;

                _threadAdatper.RunOnMainThread(() =>
                {
                    if (_effectID != ushort.MinValue)
                    {
                        EffectManager.sendUIEffect(_effectID, _effectKey, player.GetTransportConnection(), true);
                        EffectManager.sendUIEffectVisibility(_effectKey, player.GetTransportConnection(), true, "Notification", true);
                        EffectManager.sendUIEffectText(_effectKey, player.GetTransportConnection(), true, _textName, _translationsAdapter["DamageReduced"]);
                        _threadAdatper.RunOnThreadPool(async () =>
                        {
                            await Task.Delay(_effectDuration * 1000);
                            _threadAdatper.RunOnMainThread(() => EffectManager.askEffectClearByID(_effectID, player.GetTransportConnection())); 
                        });
                    }
                    else
                    {
                        ChatManager.serverSendMessage(
                            _translationsAdapter["DamageReduced", new { Percentage = damagePercent.ToString("F0") }],
                            Color.yellow,
                            toPlayer: player.channel.owner,
                            iconURL: _chatIcon
                        );
                    }
                });
            }
        }
    }
}