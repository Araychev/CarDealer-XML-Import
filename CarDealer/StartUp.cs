using CarDealer.Data;
using CarDealer.DTO.ExportDTO;
using CarDealer.DTO.ImportDTO;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            // ResetDb(context);

            // string inputXml = File.ReadAllText("./Datasets/sales.xml");
            // string result = ImportSales(context, inputXml);


            System.Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        private static void ResetDb(CarDealerContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            System.Console.WriteLine("DB Reset Success!");

        }

        private static XmlSerializer GenerateXmlSeriliazer(string rootName, Type dtoType)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);

            XmlSerializer serializer = new XmlSerializer(dtoType, xmlRoot);

            return serializer;
        }

        // 09. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Suppliers");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportSupplierDto[]), xmlRoot);

            using StringReader stringReader = new StringReader(inputXml);

            ImportSupplierDto[] dtos = (ImportSupplierDto[])serializer.Deserialize(stringReader);

            ICollection<Supplier> suppliers = new HashSet<Supplier>();

            foreach (ImportSupplierDto supplierDto in dtos)
            {
                Supplier s = new Supplier()
                {
                    Name = supplierDto.Name,
                    IsImporter = bool.Parse(supplierDto.isImporter)
                };

                suppliers.Add(s);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";

        }


        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Parts");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), xmlRoot);
            using StringReader stringReader1 = new StringReader(inputXml);
            ImportPartDto[] dtos = (ImportPartDto[])xmlSerializer.Deserialize(stringReader1);

            ICollection<Part> parts = new HashSet<Part>();

            foreach (ImportPartDto part in dtos)
            {
                Supplier supplier = context
                    .Suppliers
                    .Find(int.Parse(part.SupplierId));

                if (supplier == null)
                {
                    continue;
                }

                Part p = new Part()
                {
                    Name = part.Name,
                    Price = decimal.Parse(part.Price),
                    Quantity = int.Parse(part.Quantity),
                    Supplier = supplier

                };

                parts.Add(p);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        // 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = GenerateXmlSeriliazer("Cars", typeof(ImportCarDto[]));

            using StringReader stringReader = new StringReader(inputXml);

            ImportCarDto[] carDtos = (ImportCarDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Car> cars = new HashSet<Car>();
            //  ICollection<PartCar> partCars = new HashSet<PartCar>();

            foreach (ImportCarDto carDto in carDtos)
            {
                Car c = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TraveledDistance
                };

                ICollection<PartCar> curentCarParts = new HashSet<PartCar>();

                foreach (int partId in carDto.Parts.Select(p => p.Id).Distinct())
                {
                    Part part = context
                        .Parts
                        .Find(partId);

                    if (part == null)
                    {
                        continue;
                    }

                    PartCar partCar = new PartCar()
                    {
                        Car = c,
                        Part = part
                    };
                    curentCarParts.Add(partCar);

                }


                c.PartCars = curentCarParts;
                cars.Add(c);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";

        }


        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = GenerateXmlSeriliazer("Customers", typeof(ImportCustomersDto[]));
            using StringReader stringReader = new StringReader(inputXml);

            ImportCustomersDto[] customersDtos = (ImportCustomersDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Customer> customers = new HashSet<Customer>();

            foreach (ImportCustomersDto customerDto in customersDtos)
            {
                Customer customer = new Customer()
                {
                    Name = customerDto.Name,
                    BirthDate = DateTime.Parse(customerDto.BirthDate),
                    IsYoungDriver = bool.Parse(customerDto.IsYoungDriver)
                };
                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        // 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = GenerateXmlSeriliazer("Sales", typeof(ImportSalesDto[]));
            using StringReader stringReader = new StringReader(inputXml);

            ImportSalesDto[] salesDtos = (ImportSalesDto[])xmlSerializer.Deserialize(stringReader);
            ICollection<Sale> sales = new HashSet<Sale>();

            foreach (ImportSalesDto salesDto in salesDtos)
            {
                Car car = context
                    .Cars
                    .Find(salesDto.CarId);

                if (car == null)
                {
                    continue;
                }

                Sale sale = new Sale()
                {
                    CarId = salesDto.CarId,
                    CustomerId = salesDto.CustomerId,
                    Discount = decimal.Parse(salesDto.Discount)
                };
                sales.Add(sale);
            }
            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        //14. Export Cars With Distance

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var carsInfo = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new ExportCarDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(s => s.Make)
                .ThenBy(s => s.Model)
                .Take(10)
                .ToList();

            var serializerXml = new XmlSerializer(typeof(List<ExportCarDto>), new XmlRootAttribute("cars"));
            var xmlResult = new StringWriter();
            var nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add("", "");
            serializerXml.Serialize(xmlResult, carsInfo, nameSpaces);

            return xmlResult.ToString().Trim();
        }

        //15. Export Cars From Make BMW

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmwCars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new ExportCarBmwDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance.ToString()
                }).ToList();

            var serializerXml = new XmlSerializer(typeof(List<ExportCarBmwDto>), new XmlRootAttribute("cars"));
            var xmlResult = new StringWriter();
            var nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add("", "");
            serializerXml.Serialize(xmlResult, bmwCars, nameSpaces);

            return xmlResult.ToString().TrimEnd();
        }

        //16. Export Local Suppliers

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var supliersInfo = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportSuplierDto
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    Parts = s.Parts.Count.ToString()
                }).ToList();


            var serializerXml = new XmlSerializer(typeof(List<ExportSuplierDto>), new XmlRootAttribute("suppliers"));
            var xmlResult = new StringWriter();
            var supliersNameSpaces = new XmlSerializerNamespaces();
            supliersNameSpaces.Add("", "");
            serializerXml.Serialize(xmlResult, supliersInfo, supliersNameSpaces);
            return xmlResult.ToString().TrimEnd();
        }

        // 17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWhitParts = context.Cars
                .OrderByDescending(s => s.TravelledDistance)
                .ThenBy(s => s.Model)
                .Select(s => new ExportCarListDto
                {
                    Make = s.Make,
                    Model = s.Model,
                    TravelledDistance = s.TravelledDistance.ToString(),
                    PartCars = s.PartCars
                    .Select(p => new ExportPartDto
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    })
                    .OrderByDescending(y => y.Price)
                    .ToList()
                }).Take(5)
                .ToList();


            var serializerXml = new XmlSerializer(typeof(List<ExportCarListDto>), new XmlRootAttribute("cars"));
            var xmlResult = new StringWriter();
            var nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add("", "");
            serializerXml.Serialize(xmlResult, carsWhitParts, nameSpaces);

            return xmlResult.ToString().TrimEnd();
        }

        // 18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var totalSalesByCustomer = context.Sales
                .Where(c => c.Customer.Sales.Count > 0)
                .Select(c => new ExportCustomerSalesDto
                {
                    FullName = c.Customer.Name,
                    BoughtCars = c.Customer.Sales.Count,
                    SpentMoney = c.Car.PartCars.Sum(c => c.Part.Price)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToList();
            var serializerXml = new XmlSerializer(typeof(List<ExportCustomerSalesDto>),
                new XmlRootAttribute("customers"));
            var xmlResult = new StringWriter();
            var nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add("", "");
            serializerXml.Serialize(xmlResult, totalSalesByCustomer, nameSpaces);

            return xmlResult.ToString().TrimEnd();

        }

        // 19. Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
                var salesDiscountDto = context.Sales
            .Select(s => new ExportSalesDiscaunt()
            {
                CarDto = new ExportCarDiscountDto()
                {
                    Make = s.Car.Make,
                    Model = s.Car.Model,
                    TravelledDistance = s.Car.TravelledDistance
                },
                Discount = s.Discount,
                CustomerName = s.Customer.Name,
                Price = s.Car.PartCars.Sum(x => x.Part.Price),
                PriceWithDiscount = s.Car.PartCars.Sum(pc => pc.Part.Price) 
                    - s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100,
                })
            .ToList();

            var serializerXml = new XmlSerializer(typeof(List<ExportSalesDiscaunt>),
                                                        new XmlRootAttribute("sales"));
            var xmlResult = new StringWriter();
            var nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add("", "");
            serializerXml.Serialize(xmlResult, salesDiscountDto, nameSpaces);

            return xmlResult.ToString().Trim();
        }
    }
}