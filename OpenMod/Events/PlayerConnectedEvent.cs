using BaseGuard.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Connections.Events;
using System.Threading.Tasks;

namespace BaseGuard.OpenMod.Events
{
    public class PlayerConnectedEvent : IEventListener<UnturnedPlayerConnectedEvent>
    {
        private readonly IProtectionDecayProvider _protectionDecayProvider;
        private readonly IGroupHistoryStore _groupHistoryStore;

        public PlayerConnectedEvent(IProtectionDecayProvider protectionDecayProvider, IGroupHistoryStore groupHistoryStore)
        {
            _protectionDecayProvider = protectionDecayProvider;
            _groupHistoryStore = groupHistoryStore;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerConnectedEvent @event)
        {
            _protectionDecayProvider.DestroyTimer(@event.Player.SteamId);
            _protectionDecayProvider.DestroyTimer(@event.Player.Player.quests.groupID);

            var groupHistory = _groupHistoryStore.GetPlayerGroups(@event.Player.SteamId);
            foreach (var group in groupHistory)
            {
                _protectionDecayProvider.DestroyTimer(group);
            }

            return Task.CompletedTask;
        }
    }
}