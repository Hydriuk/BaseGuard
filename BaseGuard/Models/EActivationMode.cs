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