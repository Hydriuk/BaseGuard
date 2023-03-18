#if OPENMOD
using OpenMod.API.Ioc;
#endif

using System;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IProtectionScheduler : IDisposable
    {
        public bool IsActive { get; }
        string GetMessage();
    }
}