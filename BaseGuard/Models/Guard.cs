using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.Models
{
    public class Guard
    {
        public ushort Id { get; set; }
        public uint InstanceId { get; set; }
        public bool IsActive { get; set; }
        public float Range { get; set; }
        public float Shield { get; set; }
    }
}
