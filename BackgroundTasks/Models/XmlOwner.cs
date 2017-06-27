using System.Xml.Serialization;

namespace BackgroundTasks.Models
{
    public sealed class XmlOwner
    {
        [XmlElement("ContractorName")]
        public string ContractorName { get; set; }

        [XmlElement("LogoURL")]
        public string LogoURL { get; set; }

    }
}
