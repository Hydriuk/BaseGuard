using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.Models
{
    public class ScheduledProtection
    {
        public EState Protection { get; set; }
        public string At { get; set; } = string.Empty;
    }
}
