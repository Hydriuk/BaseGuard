using BaseGuard.API;
using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class ActiveRaidProvider : IActiveRaidProvider
    {
        private readonly IGroupHistoryStore _groupHistoryStore;
        private readonly IProtectionDecayProvider _protectionDecay;

        // FYI https://theburningmonk.com/2011/03/hashset-vs-list-vs-dictionary/
        private readonly Dictionary<CSteamID, float> _activeRaids = new Dictionary<CSteamID, float>();
        private readonly double _raidActiveTime;
        private readonly Func<CSteamID, bool> _protectGroup;

        public ActiveRaidProvider(IConfigurationAdapter<Configuration> confAdapter, IGroupHistoryStore groupHistoryStore, IProtectionDecayProvider protectionDecay)
        {
            _groupHistoryStore = groupHistoryStore;
            _protectionDecay = protectionDecay;
            _raidActiveTime = confAdapter.Configuration.RaidDuration;

            switch (confAdapter.Configuration.ProtectedGroups)
            {
                case EGroupType.NoGroup:
                    _protectGroup = groupId => groupId == CSteamID.Nil;
                    break;

                case EGroupType.InGameGroup:
                    _protectGroup = groupId => groupId.GetEAccountType() == EAccountType.k_EAccountTypeConsoleUser;
                    break;

                case EGroupType.SteamGroup:
                    _protectGroup = groupId => groupId.GetEAccountType() == EAccountType.k_EAccountTypeClan;
                    break;

                case EGroupType.Any:
                default:
                    _protectGroup = groupId => true;
                    break;
            }
        }

        public bool TryActivateRaid(CSteamID playerId, CSteamID groupId)
        {
            if (!_protectGroup(groupId))
                return true;

            if (_protectionDecay.HasProtectionDecayed(playerId))
                return true;

            float time = Time.realtimeSinceStartup;

            // Update the player's raid
            if (_activeRaids.TryGetValue(playerId, out float playerRaidTime))
            {
                if (playerRaidTime + _raidActiveTime > time)
                    Upsert(playerId);
                else
                    _activeRaids.Remove(playerId);
            }

            // Update the group's raid
            if (groupId != CSteamID.Nil && _activeRaids.TryGetValue(groupId, out float groupRaidTime))
            {
                if (groupRaidTime + _raidActiveTime > time)
                    Upsert(groupId);
                else
                    _activeRaids.Remove(groupId);
            }

            // Check if raid is already active
            if (_activeRaids.ContainsKey(playerId) || _activeRaids.ContainsKey(groupId))
                return true;

            // Try to activate the raid
            bool isGroupSet = false;
            bool isPlayerSet = false;

            foreach (var sPlayer in Provider.clients)
            {
                if (!isGroupSet && groupId != CSteamID.Nil && sPlayer.player.quests.groupID == groupId)
                {
                    _activeRaids.Add(groupId, Time.realtimeSinceStartup);
                    isGroupSet = true;
                }

                if (!isPlayerSet && sPlayer.playerID.steamID == playerId)
                {
                    _activeRaids.Add(playerId, Time.realtimeSinceStartup);
                    isPlayerSet = true;
                }

                if (!isGroupSet && _groupHistoryStore.PlayerWasInGroup(sPlayer.playerID.steamID, groupId))
                {
                    _activeRaids.Add(groupId, Time.realtimeSinceStartup);
                    isGroupSet = true;
                }

                if (isPlayerSet && isGroupSet)
                    break;
            }

            // Check if raid is active
            return isPlayerSet || isGroupSet;
        }

        private void Upsert(CSteamID steamId)
        {
            if (_activeRaids.ContainsKey(steamId))
                _activeRaids[steamId] = Time.realtimeSinceStartup;
            else
                _activeRaids.Add(steamId, Time.realtimeSinceStartup);
        }

        public bool IsRaidActivate(CSteamID steamId) => _activeRaids.ContainsKey(steamId);
    }
}