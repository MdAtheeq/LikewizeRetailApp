using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LikewizeRetailApp.Models
{
    public class CustomerDetails
    {
        public int CustomerDetailsID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}