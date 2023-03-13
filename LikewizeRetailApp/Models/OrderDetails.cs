using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LikewizeRetailApp.Models
{
    public class OrderDetails
    {
        public int OrderID { get; set; }
        public string SerialNumber { get; set; }
        public string RepairCost { get; set; }
        public string OrderDate { get; set; }
        public string Grade { get; set; }
        public string DeliveryDate { get; set; }
        public int ReceivedItemID { get; set; }
        public string ManualEstimation { get; set; }
        public string EstimatedRepairTime { get; set; }
        public string Status { get; set; }
    }
}