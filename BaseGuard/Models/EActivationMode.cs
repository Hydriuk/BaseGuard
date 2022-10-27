using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BaseGuard.Models
{
    public enum EActivationMode
    {
        [EnumMember]
        Unabled,

        [EnumMember]
        Offline,

        [EnumMember]
        Permanent
    }
}
