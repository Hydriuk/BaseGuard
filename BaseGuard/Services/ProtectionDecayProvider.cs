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
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<PlayerTime> _playerLastConnection;

        private readonly float _decayTimer;

        public ProtectionDecayProvider(IEnvironmentAdapter environmentAdapter, IConfigurationAdapter<Configuration> configuration)
        {
            _database = new LiteDatabase(Path.Combine(environmentAdapter.Directory, "playerConnections.db"));
            _playerLastConnection = _database.GetCollection<PlayerTime>("playerConnections");
            _playerLastConnection.EnsureIndex(pt => pt.PlayerId);

            _decayTimer = configuration.Configuration.ProtectionDuration;
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        public void StartTimer(CSteamID playerId)
        {
            Console.WriteLine("Start");
            _playerLastConnection.Insert(new PlayerTime()
            {
                PlayerId = playerId.m_SteamID,
                LastConnection = DateTime.Now
            });
        }

        public void DestroyTimer(CSteamID playerId)
        {
            Console.WriteLine("end");
            //_playerLastConnection.DeleteMany(pt => pt.PlayerId == playerId.m_SteamID);
        }

        public bool HasProtectionDecayed(CSteamID playerId)
        {
            if (_decayTimer < 0)
                return false;

            PlayerTime lastConnection = _playerLastConnection.FindOne(pt => pt.PlayerId == playerId.m_SteamID);

            TimeSpan protectionDuration = DateTime.Now - lastConnection.LastConnection;

            //float decayMult = 1 / Mathf.Clamp01((float)protectionDuration.TotalSeconds / _decayTimer);
            Console.WriteLine(protectionDuration.TotalHours > _decayTimer);
            return protectionDuration.TotalHours > _decayTimer;
        }
    }
}