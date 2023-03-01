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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class GroupHistoryStore : IGroupHistoryStore
    {
        private readonly IThreadAdatper _threadAdapter;

        private readonly LiteDatabase? _database;
        private readonly ILiteCollection<GroupQuit>? _groupHistory;

        private readonly Timer? _clearTimer;

        private readonly double _historyDuration;

        private bool _isClearing { get; set; }

        public GroupHistoryStore(IConfigurationAdapter<Configuration> confAdapter, IEnvironmentAdapter environmentAdapter, IThreadAdatper threadAdatper)
        {
            _threadAdapter = threadAdatper;
            _historyDuration = confAdapter.Configuration.GroupHistoryDuration;

            if (_historyDuration == 0)
                return;

            _database = new LiteDatabase(Path.Combine(environmentAdapter.Directory, "groupHistory.db"));
            _groupHistory = _database.GetCollection<GroupQuit>();

            _groupHistory.EnsureIndex(group => group.PlayerId);

            _clearTimer = new Timer(state =>
            {
                if (!_isClearing)
                {
                    _isClearing = true;
                    _threadAdapter.RunOnThreadPool(ClearHistory);
                }
            }, null, 0, 1000 * 60 * 28);
        }

        public void Dispose()
        {
            _database?.Dispose();
            _clearTimer?.Dispose();
        }

        public void ClearHistory()
        {
            _groupHistory?.DeleteMany(group => group.QuitTime.AddHours(_historyDuration) < DateTime.Now);
            _isClearing = false;
        }

        public void OnGroupQuit(CSteamID playerId, CSteamID groupId)
        {
            _groupHistory?.Insert(new GroupQuit(playerId.m_SteamID, groupId.m_SteamID, DateTime.Now));
        }

        public IEnumerable<CSteamID> GetPlayerGroups(CSteamID playerId)
        {
            return _groupHistory?
                .Find(history => history.PlayerId == playerId.m_SteamID)
                .Select(history => new CSteamID(history.GroupId)) ?? new List<CSteamID>();
        }

        public bool PlayerWasInGroup(CSteamID playerId, CSteamID groupId)
        {
            return _groupHistory?.Exists(group => group.PlayerId == playerId.m_SteamID && group.GroupId == groupId.m_SteamID) ?? false;
        }
    }
}