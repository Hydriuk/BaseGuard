using BaseGuard.API;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.Services.GuardActivators
{
    public class PermanentGuardActivator : IGuardActivator
    {
        public bool IsGuardActive(CSteamID steamID) => true;

        public bool TryActivateGuard(CSteamID playerId, CSteamID groupId) => true;
    }
}
