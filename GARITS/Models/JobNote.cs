using System;
namespace GARITS.Models
{
    public class JobNote
    {
        public string id { get; set; }
        public string body { get; set; }
        public string type { get; set; }
        public DateTime time { get; set; }
        public User user { get; set; }

    }

}
