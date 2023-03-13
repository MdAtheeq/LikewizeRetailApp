using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LikewizeRetailApp.Models
{
    public class ReceivedItems
    {
        public int ReceivedItemID { get; set; }
        public int ProductDetailsID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string SerialNumber { get; set; }
        public string ProductType { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Status { get; set; }
        public string DateReceived { get; set; }
        public int Count { get; set; }

    }
}