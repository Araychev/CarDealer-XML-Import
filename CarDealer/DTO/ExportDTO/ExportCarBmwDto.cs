using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDTO
{
     [XmlType("car")]

    public class ExportCarBmwDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public string TravelledDistance { get; set; }
    
    }
}
