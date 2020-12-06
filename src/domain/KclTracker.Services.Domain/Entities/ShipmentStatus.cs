using System;
using System.Collections.Generic;
using System.Text;

namespace KclTracker.Services.Domain.Entities
{
    public class ShipmentStatus
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int OrderId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
