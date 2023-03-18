using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BaseGuard.Models
{
    public class ScheduledProtection
    {
        [XmlAttribute]
        public EState Protection { get; set; }

        [XmlAttribute]
        public string At { get; set; } = string.Empty;
    }
}
