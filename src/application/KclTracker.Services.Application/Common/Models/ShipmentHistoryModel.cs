using System;
using System.Collections.Generic;
using System.Text;

namespace KclTracker.Services.Application.Common.Models
{
    public class ShipmentHistoryModel
    {
        public int OrderId { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string Status { get; set; }
    }
}
