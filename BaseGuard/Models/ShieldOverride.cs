using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.Models
{
    public class ShieldOverride
    {
        public ushort Id { get; set; }
        public float BaseShield { get; set; }
        public float MaxShield { get; set; }
    }
}
