using System;
namespace GARITS.Models
{
    public class Customer
    {

        public string customerID { get; set; }
        public string email { get; set; }
        public DateTime registered { get; set; }
        public string title { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string addressline1 { get; set; }
        public string addressline2 { get; set; }
        public string county { get; set; }
        public string postcode { get; set; }
        public string phone { get; set; }

    }
}
