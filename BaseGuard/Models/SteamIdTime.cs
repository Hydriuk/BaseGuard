using System;

namespace BaseGuard.Models
{
    public class SteamIdTime
    {
        public ulong SteamId { get; set; }
        public DateTime LastConnection { get; set; }
    }
}