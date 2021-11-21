
using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDTO
{
    [XmlType("suplier")]
    public class ExportSuplierDto
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("parts-count")]
        public string Parts { get; set; }
    }
}
