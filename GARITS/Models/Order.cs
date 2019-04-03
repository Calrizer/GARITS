using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GARITS.Models
{
    public class Order
    {
        public string orderID { get; set; }
        public DateTime date { get; set; }
        public DateTime deliveredDate { get; set; }
        public string addressline1 { get; set; }
        public string addressline2 { get; set; }
        public string town { get; set; }
        public string county { get; set; }
        public string postcode { get; set; }
        public string username { get; set; }
    }
}
