using KclTracker.Services.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KclTracker.Services.Application.Shipments.Queries.GetShipmentDetail
{
    public class ShipmentDetailVm
    {
        public string DeclarationNumber { get; set; }

        public string CdnNumber { get; set; }

        public string PartName { get; set; }

        public string CompanyName { get; set; }

        public DateTime DateOfEntry { get; set; }

        public string InspectionType { get; set; }

        public string ContainerNumbers { get; set; }

        public int NumberOfContainers { get; set; }

        public IEnumerable<ShipmentHistoryModel> History { get; set; }
    }
}
