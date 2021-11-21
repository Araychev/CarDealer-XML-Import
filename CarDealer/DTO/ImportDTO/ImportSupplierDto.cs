using System.Xml.Serialization;

namespace CarDealer.DTO.ImportDTO
{
    [XmlType("Supplier")]
    public class ImportSupplierDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("isImporter")]
        public string isImporter { get; set; }

    }
}
