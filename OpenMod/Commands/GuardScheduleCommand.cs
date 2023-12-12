using BaseGuard.API;
using Cysharp.Threading.Tasks;
using Hydriuk.Unturned.SharedModules.Adapters;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using UnityEngine;

namespace BaseGuard.OpenMod.Commands
{
    [Command("protectionschedule")]
    [CommandAlias("protsch")]
    [CommandDescription("Displays the current protection state and the remaining time")]
    public class GuardScheduleCommand : UnturnedCommand
    {
        private readonly IProtectionScheduler _protectionScheduler;
        private readonly IThreadAdatper _threadAdatper;
        private readonly string _chatIcon;

        public GuardScheduleCommand(
            IServiceProvider serviceProvider,
            IProtectionScheduler protectionScheduler,
            IConfigurationAdapter<Configuration> configuration,
            IThreadAdatper threadAdatper) : base(serviceProvider)
        {
            _protectionScheduler = protectionScheduler;
            _threadAdatper = threadAdatper;
            _chatIcon = configuration.Configuration.ChatMessages.ChatIcon;
        }

        protected override UniTask OnExecuteAsync()
        {
            string message = _protectionScheduler.GetMessage();

            if (string.IsNullOrEmpty(message))
                return UniTask.CompletedTask;

            if (Context.Actor is UnturnedUser user)
            {
                _threadAdatper.RunOnMainThread(() =>
                {
                    ChatManager.serverSendMessage(
                        message,
                        Color.green,
                        toPlayer: user.Player.SteamPlayer,
                        iconURL: _chatIcon
                    );
                });
            }
            else
            {
                Console.WriteLine(message);
            }

            return UniTask.CompletedTask;
        }
    }
}