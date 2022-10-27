using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BaseGuard.Models
{
    public enum EGuardMode
    {
        [EnumMember]
        Base,

        [EnumMember]
        Cumulative,

        [EnumMember]
        Ratio,

        [EnumMember]
        All
    }
}
