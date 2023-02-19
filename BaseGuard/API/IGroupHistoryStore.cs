#if OPENMOD
using OpenMod.API.Ioc;
#endif
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IGroupHistoryStore : IDisposable
    {
        void ClearHistory();
        void OnGroupQuit(CSteamID playerId, CSteamID groupId);
        bool PlayerWasInGroup(CSteamID playerId, CSteamID groupId);
    }
}
