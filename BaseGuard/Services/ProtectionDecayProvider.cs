using BaseGuard.API;
using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using LiteDB;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Steamworks;
using System;
using System.IO;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class ProtectionDecayProvider : IProtectionDecayProvider
    {
        private readonly LiteDatabase? _database;
        private readonly ILiteCollection<SteamIdTime>? _lastConnection;

        private readonly double _decayTimer;

        public ProtectionDecayProvider(IEnvironmentAdapter environmentAdapter, IConfigurationAdapter<Configuration> configuration)
        {
            _decayTimer = configuration.Configuration.ProtectionDuration;

            if (_decayTimer <= 0)
                return;

            _database = new LiteDatabase(Path.Combine(environmentAdapter.Directory, "lastConnections.db"));
            _lastConnection = _database.GetCollection<SteamIdTime>("lastConnections");
            _lastConnection.EnsureIndex(pt => pt.SteamId);
        }

        public void Dispose()
        {
            _database?.Dispose();
        }

        public void StartTimer(CSteamID steamId)
        {
            _lastConnection?.Insert(new SteamIdTime()
            {
                SteamId = steamId.m_SteamID,
                LastConnection = DateTime.Now
            });
        }

        public void DestroyTimer(CSteamID steamId)
        {
            _lastConnection?.DeleteMany(pt => pt.SteamId == steamId.m_SteamID);
        }

        public bool HasProtectionDecayed(CSteamID steamId)
        {
            if (_lastConnection == null)
                return false;

            SteamIdTime lastConnection = _lastConnection.FindOne(pt => pt.SteamId == steamId.m_SteamID);

            if (lastConnection == null)
                return true;

            TimeSpan protectionDuration = DateTime.Now - lastConnection.LastConnection;

            //float decayMult = 1 / Mathf.Clamp01((float)protectionDuration.TotalSeconds / _decayTimer);

            return protectionDuration.TotalHours > _decayTimer;
        }
    }
}