using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.API
{
    public interface IGuardActivator
    {
        bool IsGuardActive(CSteamID steamID);
        bool TryActivateGuard(CSteamID playerId, CSteamID groupId);
    }
}
