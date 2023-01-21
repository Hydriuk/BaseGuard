using Steamworks;

namespace BaseGuard.API
{
    public interface IGuardActivator
    {
        /// <summary>
        /// Check if owner's guards are activated
        /// </summary>
        /// <param name="steamID"> Owner to check </param>
        /// <returns> True if guards are activated, false otherwise </returns>
        bool IsGuardActive(CSteamID steamID);

        /// <summary>
        /// Activate guards
        /// </summary>
        /// <param name="playerId"> Player of whom to activate guards </param>
        /// <param name="groupId"> Group of whom to activate guards </param>
        /// <returns> True if guards are activated, false otherwise </returns>
        bool TryActivateGuard(CSteamID playerId, CSteamID groupId);
    }
}