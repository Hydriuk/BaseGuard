#if OPENMOD
using OpenMod.API.Ioc;
#endif
using Steamworks;
using System;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IProtectionDecayProvider : IDisposable
    {
        void StartTimer(CSteamID playerId);
        void DestroyTimer(CSteamID playerId);
        bool HasProtectionDecayed(CSteamID playerId);
    }
}