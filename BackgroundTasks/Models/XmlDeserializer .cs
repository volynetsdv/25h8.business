using System.Xml.Serialization;

namespace BackgroundTasks.Models
{
    public sealed class XmlDeserializer
    {
        [XmlElement("EntityType")]
        public string EntityType { get; set; }

        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("Process")]
        public string Process { get; set; }

        [XmlElement("Id")]
        public string Id { get; set; }

        [XmlElement("State")]
        public string State { get; set; }

        [XmlElement("Owner")]
        public XmlOwner XmlOwner { get; set; }
    }
    
}
