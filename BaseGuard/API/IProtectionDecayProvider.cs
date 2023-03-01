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
        void StartTimer(CSteamID steamId);
        void DestroyTimer(CSteamID steamId);
        bool HasProtectionDecayed(CSteamID steamId);
    }
}