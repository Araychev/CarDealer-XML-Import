
using System.Xml.Serialization;

namespace CarDealer.DTO.ImportDTO
{
    [XmlType("partId")]
    public class ImportCarPartDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
