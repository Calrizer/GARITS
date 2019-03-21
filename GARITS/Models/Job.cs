using System;
using System.Collections.Generic;


namespace GARITS.Models
{
    public class Job
    {
        public string jobID { get; set; }
        public DateTime start { get; set; }
        public DateTime? end { get; set; }
        public DateTime? paid { get; set; }
        public int bay { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public Customer customer { get; set; }
        public Vehicle vehicle { get; set; }
        public List<JobNote> notes { get; set; }
        public Dictionary<Part, int> parts { get; set; }
        public Dictionary<string, float> labour { get; set; }
        public User mechanic { get; set; }

    }
}
