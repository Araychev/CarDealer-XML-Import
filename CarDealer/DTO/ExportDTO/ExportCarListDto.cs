

using System.Collections.Generic;
using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDTO
{
     [XmlType("car")]
    public class ExportCarListDto
    {
       [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public string TravelledDistance { get; set; }

        [XmlArray("parts")]
        public List<ExportPartDto> PartCars { get; set; }
    }
}
