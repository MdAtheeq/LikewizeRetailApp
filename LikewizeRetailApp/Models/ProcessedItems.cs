using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LikewizeRetailApp.Models
{
    public class ProcessedItems
    {
        public int ProcessedItemID { get; set; }
        public string SerialNumber { get; set; }
        public int DefectDetailsID { get; set; }
        public int ReceivedItemID { get; set; }
        public string DateProcessed { get; set; }
        public string Grade { get; set; }
        public string RepairCost { get; set; }
        public string Status { get; set; }
        public int Count { get; set; }

    }
}