using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace BaseGuard.Models
{
    public enum EActivationMode
    {
        [XmlEnum]
        Unabled,

        [XmlEnum]
        Offline,

        [XmlEnum]
        Permanent
    }
}
