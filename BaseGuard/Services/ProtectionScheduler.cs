﻿using BaseGuard.API;
using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class ProtectionScheduler : IProtectionScheduler
    {
        public bool IsActive { get; private set; } = true;

        private readonly ITranslationsAdapter _translationsAdapter;
        private readonly IThreadAdatper _threadAdatper;

        private readonly string _chatIcon;
        private readonly ushort _effectID;
        private readonly string _textName;
        private readonly int _effectDuration;
        private short _effectKey { get => (short)(_effectID - short.MaxValue); }

        private readonly List<Pattern> _patterns = new List<Pattern>();
        private readonly Timer _timer = new Timer(1000);

        private Pattern _nextPattern = new Pattern("0 0 1 1 0", EState.On);

        public ProtectionScheduler(IConfigurationAdapter<Configuration> configuration, ITranslationsAdapter translationsAdapter, IThreadAdatper threadAdatper)
        {
            _translationsAdapter = translationsAdapter;
            _threadAdatper = threadAdatper;

            _chatIcon = configuration.Configuration.ChatMessages.ChatIcon;
            _effectID = configuration.Configuration.ChatMessages.EffectID;
            _textName = configuration.Configuration.ChatMessages.EffectTextName;
            _effectDuration = configuration.Configuration.ChatMessages.EffectDuration;

            foreach (var timePattern in configuration.Configuration.Schedule)
            {
                _patterns.Add(new Pattern(timePattern.At, timePattern.Protection));
            }

            _timer.Elapsed += TimerElapsed;

            if (_patterns.Count > 0)
            {
                // Get max pattern by PreviousOccurence
                Pattern lastPattern = _patterns[0];
                foreach (var pattern in _patterns)
                {
                    if (pattern.PreviousOccurence > lastPattern.PreviousOccurence)
                        lastPattern = pattern;
                }

                IsActive = lastPattern.State;

                UpdatePattern();
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public string GetMessage()
        {
            TimeSpan timeLeft = _nextPattern.NextOccurence - DateTime.Now;

            if (_patterns.Count == 0)
            {
                string message = _translationsAdapter["NoProtectionSchedule"];
                if (message != "NoProtectionSchedule")
                    return message;
                else
                    return string.Empty;
            }

            // Round to seconds
            int seconds = (int)Math.Round(timeLeft.TotalSeconds);
            timeLeft = new TimeSpan(0, 0, seconds);

            if (IsActive && _translationsAdapter["ProtectionActivated"] != "ProtectionActivated")
            {
                return _translationsAdapter["ProtectionActivated", GetTimes(timeLeft)];
            }

            if (!IsActive && _translationsAdapter["ProtectionDeactivated"] != "ProtectionActivated")
            {
                return _translationsAdapter["ProtectionDeactivated", GetTimes(timeLeft)];
            }

            return string.Empty;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            IsActive = _nextPattern.State;

            _nextPattern.UpdateNextOccurence();
            UpdatePattern();

            string message = GetMessage();

            if (!string.IsNullOrEmpty(message))
            {
                _threadAdatper.RunOnMainThread(async () =>
                {
                    if (_effectID != ushort.MinValue)
                    {
                        EffectManager.sendUIEffect(_effectID, _effectKey, true);
                        Provider.GatherClientConnections().ForEach(conn => EffectManager.sendUIEffectText(_effectKey, conn, true, _textName, message));
                        _threadAdatper.RunOnThreadPool(async () =>
                        {
                            await Task.Delay(_effectDuration * 1000);
                            _threadAdatper.RunOnMainThread(() => EffectManager.ClearEffectByID_AllPlayers(_effectID));
                        });
                    }
                    else
                    {
                        ChatManager.serverSendMessage(
                            message,
                            Color.green,
                            iconURL: _chatIcon
                        );
                    }
                });
            }
        }

        private void UpdatePattern()
        {
            // Get min pattern by NextOccurence
            foreach (var pattern in _patterns)
            {
                if(pattern.NextOccurence < _nextPattern.NextOccurence)
                    _nextPattern =  pattern;
            }

            TimeSpan waitTime = _nextPattern.NextOccurence - DateTime.Now;
            if (waitTime <= TimeSpan.Zero)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(-waitTime);
                    UpdatePattern();
                });
                return;
            }

            _timer.Interval = waitTime.TotalMilliseconds + 200;

            _timer.Start();
        }

        private static object GetTimes(TimeSpan time)
        {
            return new
            {
                ExactDays = Math.Round(time.TotalHours, 2),
                ExactHours = Math.Round(time.TotalHours, 2),
                ExactMinutes = Math.Round(time.TotalMinutes, 2),
                TotalHours = Math.Floor(time.TotalHours),
                TotalMinutes = Math.Floor(time.TotalMinutes),
                TotalSeconds = Math.Floor(time.TotalSeconds),
                Days = Math.Floor(time.TotalDays),
                Hours = time.Hours,
                Minutes = time.Minutes,
                Seconds = time.Seconds
            };
        }

        private class Pattern
        {
            public bool State { get; set; }
            public DateTime NextOccurence { get; private set; }
            public DateTime PreviousOccurence { get => GetPreviousOccurence(); }

            private readonly CrontabSchedule _scheduler;

            public Pattern(string scheduler, EState state)
            {
                _scheduler = CrontabSchedule.Parse(scheduler);
                State = state switch
                {
                    EState.On => true,
                    EState.Off => false,
                    _ => true
                };

                UpdateNextOccurence();
            }

            public void UpdateNextOccurence()
            {
                NextOccurence = _scheduler.GetNextOccurrence(DateTime.Now);
            }

            public DateTime GetPreviousOccurence()
            {
                DateTime lastOccurence;

                lastOccurence = _scheduler.GetNextOccurrences(DateTime.Now.AddHours(-1), DateTime.Now).LastOrDefault();
                if (lastOccurence != null && lastOccurence != DateTime.MinValue) return lastOccurence;

                lastOccurence = _scheduler.GetNextOccurrences(DateTime.Now.AddDays(-1), DateTime.Now).LastOrDefault();
                if (lastOccurence != null && lastOccurence != DateTime.MinValue) return lastOccurence;

                lastOccurence = _scheduler.GetNextOccurrences(DateTime.Now.AddMonths(-1), DateTime.Now).LastOrDefault();
                if (lastOccurence != null && lastOccurence != DateTime.MinValue) return lastOccurence;

                lastOccurence = _scheduler.GetNextOccurrences(DateTime.Now.AddYears(-2), DateTime.Now).LastOrDefault();
                if (lastOccurence != null && lastOccurence != DateTime.MinValue) return lastOccurence;

                return DateTime.MinValue;
            }
        }
    }
}