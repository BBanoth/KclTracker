using System;
using System.Collections.Generic;
using System.Text;

namespace KclTracker.Services.Domain.Entities
{
    public class Shipment
    {
        public int Id { get; set; }

        public string DeclarationNumber { get; set; }

        public string CdnNumber { get; set; }

        public int CompanyId { get; set; }

        public string PartName { get; set; }

        public DateTime DateOfEntry { get; set; }

        public int InspectionTypeId { get; set; }

        public string ContainerNumbers { get; set; }

        public int NumberOfContainers { get; set; }

        public InspectionType InspectionType { get; set; }

        public ICollection<ShipmentHistory> History { get; set; }
    }
}
