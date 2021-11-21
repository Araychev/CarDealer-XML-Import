
using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDTO
{
    [XmlType("customer")]
    public class ExportCustomerSalesDto
    {
         [XmlAttribute("full-name")]
        public string FullName { get; set; }

        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }

        [XmlAttribute("spent-money")]
        public decimal SpentMoney { get; set; }
    }
}
