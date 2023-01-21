using System.Xml.Serialization;

namespace BaseGuard.Models
{
    public enum EGuardMode
    {
        [XmlEnum]
        Base,

        [XmlEnum]
        Cumulative,

        [XmlEnum]
        Ratio
    }
}