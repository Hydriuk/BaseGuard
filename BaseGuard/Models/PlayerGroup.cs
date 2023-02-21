using System;

namespace BaseGuard.Models
{
    public class GroupQuit
    {
        public ulong PlayerId { get; set; }
        public ulong GroupId { get; set; }
        public DateTime QuitTime { get; set; }

        public GroupQuit(ulong playerId, ulong groupId, DateTime quitTime)
        {
            PlayerId = playerId;
            GroupId = groupId;
            QuitTime = quitTime;
        }

        public GroupQuit()
        {
        }
    }
}