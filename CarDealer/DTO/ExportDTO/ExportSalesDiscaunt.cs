﻿
using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDTO
{
     [XmlType("sale")]
    public class ExportSalesDiscaunt
    {
         [XmlElement("car")]
        public ExportCarDiscountDto CarDto { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }
    }
}