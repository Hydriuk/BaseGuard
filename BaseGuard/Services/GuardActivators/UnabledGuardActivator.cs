using BaseGuard.API;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.Services.GuardActivators
{
    public class UnabledGuardActivator : IGuardActivator
    {
        public bool IsGuardActive(CSteamID steamID) => false;

        public bool TryActivateGuard(CSteamID playerId, CSteamID groupId) => false;
    }
}
