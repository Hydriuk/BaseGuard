using BaseGuard.API;
using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using MoreLinq;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;
using SmartFormat.Core.Formatting;
using System.Reflection;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class ProtectionScheduler : IProtectionScheduler, IDisposable
    {
        public bool IsActive { get; private set; } = true;

        private ITranslationsAdapter _translationsAdapter;
        private IThreadAdatper _threadAdatper;
        private string _chatIcon;

        private readonly List<Pattern> _patterns = new List<Pattern>();
        private Timer _timer = new Timer(1000);
        private Pattern _nextPattern = new Pattern("0 0 1 1 0", EState.On);

        public ProtectionScheduler(IConfigurationAdapter<Configuration> configuration, ITranslationsAdapter translationsAdapter, IThreadAdatper threadAdatper)
        {
            _translationsAdapter = translationsAdapter;
            _threadAdatper = threadAdatper;
            _chatIcon = configuration.Configuration.ChatIcon;

            foreach (var timePattern in configuration.Configuration.Schedule)
            {
                _patterns.Add(new Pattern(timePattern.At, timePattern.Protection));
            }

            _timer.Elapsed += TimerElapsed;

            if (_patterns.Count > 0)
            {
                Pattern lastPattern = _patterns.MaxBy(pattern => pattern.PreviousOccurence).LastOrDefault();
                IsActive = lastPattern.State;

                _timer.Start();
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public string GetMessage()
        {
            TimeSpan timeLeft = _nextPattern.NextOccurence - DateTime.Now;

            // Round to seconds
            int seconds = (int)Math.Round(timeLeft.TotalSeconds);
            timeLeft = new TimeSpan(0, 0, seconds);

            try
            {
                if (IsActive && !string.IsNullOrWhiteSpace(_translationsAdapter["ProtectionActivated"]))
                {
                    return _translationsAdapter["ProtectionActivated", GetTimes(timeLeft)];
                }
                else if (!IsActive && !string.IsNullOrWhiteSpace(_translationsAdapter["ProtectionDeactivated"]))
                {
                    return _translationsAdapter["ProtectionDeactivated", GetTimes(timeLeft)];
                }
            }
            catch(FormattingException e)
            {
                PropertyInfo[] timeProperties = GetTimes(timeLeft).GetType().GetProperties();
                string propertiesString = timeProperties
                    .Select(property => property.Name)
                    .Aggregate((acc, curr) => $"{acc} {curr}");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"[{DateTime.Now.ToString("T")}][BaseGuard] - {e.Message}");
                Console.WriteLine($"[BaseGuard] - Please, review your BaseGuard translations file.");
                Console.WriteLine($"[BaseGuard] - One or more of the parameters is wrongly written.");
                Console.WriteLine($"[BaseGuard] - Available parameters are : {propertiesString}");
                Console.ResetColor();
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
                _threadAdatper.RunOnMainThread(() =>
                {
                    ChatManager.serverSendMessage(
                        message,
                        Color.green,
                        iconURL: _chatIcon
                    );
                });
            }
        }

        private void UpdatePattern()
        {
            _nextPattern = _patterns.MinBy(scheduler => scheduler.NextOccurence).FirstOrDefault();

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
                RoundedHours = Math.Round(time.TotalHours, 2),
                RoundedMinutes = Math.Round(time.TotalMinutes, 2),
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
            private CrontabSchedule _scheduler { get; set; }
            public bool State { get; set; }
            public DateTime NextOccurence { get; private set; }
            public DateTime PreviousOccurence { get => GetPreviousOccurence(); }

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
                if (lastOccurence != null) return lastOccurence;

                lastOccurence = _scheduler.GetNextOccurrences(DateTime.Now.AddDays(-1), DateTime.Now).LastOrDefault();
                if (lastOccurence != null) return lastOccurence;

                lastOccurence = _scheduler.GetNextOccurrences(DateTime.Now.AddMonths(-1), DateTime.Now).LastOrDefault();
                if (lastOccurence != null) return lastOccurence;

                lastOccurence = _scheduler.GetNextOccurrences(DateTime.Now.AddYears(-1), DateTime.Now).LastOrDefault();
                if (lastOccurence != null) return lastOccurence;

                lastOccurence = _scheduler.GetNextOccurrences(DateTime.Now.AddYears(-12), DateTime.Now).LastOrDefault();
                if (lastOccurence != null) return lastOccurence;

                return DateTime.MinValue;
            }
        }
    }
}