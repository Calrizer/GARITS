using System;
using System.Collections.Generic;
using System.Globalization;
using GARITS.Models;
using GARITS.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GARITS.Controllers
{
    public class ReportController : Controller
    {
        // GET
        public IActionResult ViewStockReport(string start, string end)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            List<Job> jobs = JobProvider.getAllJobs("COMPLETE", start, end);

            List<Part> parts = PartsProvider.getParts();
            
            Dictionary<Part, int> orders = PartsProvider.getAllOrders();
            
            Dictionary<string, int> used = new Dictionary<string, int>();

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

            }
            
            ViewData["Start"] = DateTime.ParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            ViewData["End"] = DateTime.ParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            ViewData["Parts"] = parts;
            ViewData["Used"] = used;

            return View("ViewStockReport");

        }
        
        private bool isAuthenticated()
        {

            if (HttpContext.Session.GetString("user") != null)
            {

                return true;

            }

            return false;

        }
        
        public User getAuthenticatedUser()
        {

            return UserProvider.getUserFromUsername(HttpContext.Session.GetString("user"));

        }
        
    }
    
    
}