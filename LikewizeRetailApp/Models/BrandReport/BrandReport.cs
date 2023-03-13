using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace LikewizeRetailApp.Models.BrandReport
{
    //[DataContract]
    public class BrandReport
    {
        //[DataMember(Name ="y")]
        public int Count { get; set; }

        //[DataMember(Name ="x")]
        public string Brand { get; set; }
    }
}