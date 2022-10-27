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
    public interface IGuardActivator
    {
        bool IsGuardActive(CSteamID steamID);
        bool TryActivateGuard(CSteamID playerId, CSteamID groupId);
    }
}
