using System.Collections.Generic;
using System.Configuration;
using GARITS.Providers;
using GARITS.Models;

namespace GARITS.Providers
{
    public static class ReportProvider
    {
        
        private static string connection = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        public static void generateStockReport(string start, string end)
        {

            List<Job> jobs = JobProvider.getAllJobs("COMPLETE", start, end);

            List<Part> parts = PartsProvider.getParts();
            
            Dictionary<Part, int> orders = PartsProvider.getAllOrders();
            
            Dictionary<string, int> used = new Dictionary<string, int>();
            Dictionary<string, int> ordered = new Dictionary<string, int>();

            foreach (Part part in parts)
            {

                foreach (Job job in jobs)
                {

                    foreach (KeyValuePair<Part, int> allocation in job.parts)
                    {

                        if (part.partID == allocation.Key.partID)
                        {

                            if (!used.ContainsKey(part.partID))
                            {
                                
                                used.Add(part.partID, allocation.Value);
                                
                            }
                            else
                            {

                                used[part.partID] = used[part.partID] + allocation.Value;

                            }
                            
                        }
                        
                    }
                    
                }

                foreach (KeyValuePair<Part, int> allocation in orders)
                {
                    
                    if (part.partID == allocation.Key.partID)
                    {

                        if (!ordered.ContainsKey(part.partID))
                        {
                                
                            ordered.Add(part.partID, allocation.Value);
                                
                        }
                        else
                        {

                            ordered[part.partID] = ordered[part.partID] + allocation.Value;

                        }
                            
                    }
                    
                }

            }

        }
        
    }
    
}