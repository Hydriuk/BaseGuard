using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.Models
{
    public class PlayerTime
    {
        public ulong PlayerId { get; set; }
        public DateTime LastConnection { get; set; }
    }
}
