using System;
namespace GARITS.Models
{
    public class Part
    {

        public string partID { get; set; }
        public string name { get; set; }
        public string manufacturer { get; set; }
        public string vehicle { get; set; }
        public string years { get; set; }
        public float price { get; set; }
        public int quantity { get; set; }
        public int threshold { get; set; }

    }
}
