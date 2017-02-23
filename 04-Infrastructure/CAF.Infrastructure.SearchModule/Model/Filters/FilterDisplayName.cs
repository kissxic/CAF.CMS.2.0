using System.Xml.Serialization;

namespace CAF.Infrastructure.SearchModule.Model.Filters
{
    public class FilterDisplayName
    {
        [XmlAttribute("language")]
        public string Language { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
