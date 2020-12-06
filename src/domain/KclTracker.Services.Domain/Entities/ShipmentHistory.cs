using System;
using System.Collections.Generic;
using System.Text;

namespace KclTracker.Services.Domain.Entities
{
    public class ShipmentHistory
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }

        public int ShipmentStatusId { get; set; }

        public ShipmentStatus Status { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
