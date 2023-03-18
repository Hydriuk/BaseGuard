using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BaseGuard.RocketMod.Commands
{
    public class GuardSchedulerCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "protectionschedule";

        public string Help => "Displays the current protection state and the remaining time";

        public string Syntax => string.Empty;

        public List<string> Aliases => new List<string>() { "protsch" };

        public List<string> Permissions => new List<string>() { "baseguard.protectionschedule" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is UnturnedPlayer player)
            {
                Plugin.Instance.ThreadAdapter.RunOnMainThread(() =>
                {
                    ChatManager.serverSendMessage(
                        Plugin.Instance.ProtectionScheduler.GetMessage(),
                        Color.green,
                        toPlayer: player.SteamPlayer(),
                        iconURL: Plugin.Instance.ConfigurationAdapter.Configuration.ChatIcon
                    );
                });
            }
            else
            {
                Console.WriteLine(Plugin.Instance.ProtectionScheduler.GetMessage());
            }
        }
    }
}
