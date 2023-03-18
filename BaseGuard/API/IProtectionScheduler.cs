#if OPENMOD
using OpenMod.API.Ioc;
#endif

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IProtectionScheduler
    {
        public bool IsActive { get; }
        string GetMessage();
    }
}