#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;

namespace BaseGuard.API
{
#if OPENMOD
    [Service]
#endif
    public interface IDamageWarner
    {
        void TryWarn(Player player, float oldDamage, float newDamage);
    }
}