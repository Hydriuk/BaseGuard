using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaseGuard.Models
{
    public class ShieldOverride
    {
        [XmlArrayItem("Id")]
        public List<ushort> Ids { get; set; } = new List<ushort>();
        public float BaseShield { get; set; }
        public float MaxShield { get; set; }
    }
}