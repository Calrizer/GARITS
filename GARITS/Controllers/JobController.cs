using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GARITS.Models;
using GARITS.Providers;

namespace GARITS.Controllers
{
    public class JobController : Controller
    {
        // GET: /<controller>/

        public IActionResult ViewJob(string id)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            string jobID = id;

            Job job = JobProvider.getJobDetails(jobID);

            ViewData["JobDetails"] = job;
            ViewData["Previous"] = JobProvider.getPreviousJobs(job.vehicle.vrm, jobID);

            return View("Job");

        }

        public IActionResult ViewJobs(string filter, string start, string end)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Filter"] = filter;
            ViewData["Start"] = DateTime.ParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            ViewData["End"] = DateTime.ParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            
            ViewData["Jobs"] = JobProvider.getAllJobs(filter, start, end);
            
            return View("View");

        }

        public IActionResult Book()
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            return View("Book");

        }

        [HttpPost]
        public IActionResult BookVehicle(string type, string issue, string cosmetic, string resolution, string labour, string bay, string customerID, string vrm, string mechanic)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            JobNote estimate = new JobNote
            {

                id = JobProvider.GetUniqueKey(255),
                type = "Estimate",
                body = "Issue: " + issue + " Cosmetic Condition: " + cosmetic + " Proposed Resolution: " + resolution,
                time = DateTime.Now,
                user = UserProvider.getUserFromUsername(HttpContext.Session.GetString("user"))

            };

            JobNote labourEstimate = new JobNote
            {

                id = JobProvider.GetUniqueKey(255),
                type = "Labour Estimate",
                body = labour,
                time = DateTime.Now,
                user = UserProvider.getUserFromUsername(HttpContext.Session.GetString("user"))

            };

            List<JobNote> notes = new List<JobNote>();
            notes.Add(estimate);
            notes.Add(labourEstimate);

            Job job = new Job
            {

                jobID = JobProvider.GetUniqueNumber(8),
                start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day),
                end = null,
                paid = null,
                bay = Int32.Parse(bay),
                status = "Ongoing",
                type = type,
                customer = CustomerProvider.getCustomerFromID(customerID),
                vehicle = VehicleProvider.getVehicleFromVRM(vrm),
                notes = notes,
                labour = new Dictionary<string, float>(),
                mechanic = UserProvider.getUserFromUsername(mechanic)

            };

            JobProvider.createJob(job);

            TempData["JobID"] = job.jobID;

            return RedirectToAction("ViewJob", new { id = job.jobID });

        }

        public IActionResult BookAddVehicle(string vrm)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            Vehicle vehicle = VehicleProvider.getVehicleFromVRM(vrm);

            if (vehicle.vrm == null) return RedirectToAction("AddVehicle", "Vehicle", new {vrm = vrm});

            ViewData["Vehicle"] = vehicle;
            Customer customer = VehicleProvider.getCustomerFromVehicle(vrm);
            ViewData["Customer"] = customer;
            ViewData["Debt"] = CustomerProvider.checkIfInDebt(customer.customerID);
            ViewData["Mechanic"] = UserProvider.getUsers();

            return View("Book");

        }

        public IActionResult Invoice(string jobID)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["InvoiceNote"] = JobProvider.getInvoiceNote(jobID);
            ViewData["JobDetails"] = JobProvider.getJobDetails(jobID);

            return View("Invoice");

        }
        
        public IActionResult Reminder(string jobID, string reminder)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["JobDetails"] = JobProvider.getJobDetails(jobID);
            ViewData["Reminder"] = Int32.Parse(reminder);
            
            return View("Reminder");

        }

        public IActionResult JobSheet(string jobID)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["EstimateNote"] = JobProvider.getEstimateNote(jobID);
            ViewData["JobDetails"] = JobProvider.getJobDetails(jobID);
            
            return View("JobSheet");

        }

        [HttpPost]
        public IActionResult AddNote(string type, string body, string jobID)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            JobNote note = new JobNote
            {

                id = JobProvider.GetUniqueKey(255),
                type = type,
                body = body,
                time = DateTime.Now,
                user = UserProvider.getUserFromUsername(HttpContext.Session.GetString("user"))

            };

            JobProvider.addJobNote(note, jobID);

            if (type == "Invoice")
            {
                JobProvider.updateStatus(jobID, "Complete - Awaiting Payment");
            }

            return RedirectToAction("ViewJob", "Job", new { id = jobID });

        }

        [HttpPost]
        public IActionResult Pay(string jobID)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            JobProvider.updateStatus(jobID, "Complete - Paid");
            
            JobNote note = new JobNote
            {

                id = JobProvider.GetUniqueKey(255),
                type = "Payment",
                body = "Customer payment taken.",
                time = DateTime.Now,
                user = UserProvider.getUserFromUsername(HttpContext.Session.GetString("user"))

            };
            
            JobProvider.addJobNote(note, jobID);
            
            return RedirectToAction("ViewJob", "Job", new { id = jobID });
            
        }
        
        [HttpPost]
        public IActionResult AddPart(string jobID, string search)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            List<Part> parts = new List<Part>();

            Dictionary<Part, int> existing = JobProvider.getPartsForJob(jobID);
            
            String[] terms = search.Split(" ");

            foreach (String term in terms)
            {
                foreach (Part result in PartsProvider.searchForParts(term))
                {

                    bool contains = false;
                    
                    foreach (Part check in parts)
                    {

                        if (check.partID == result.partID) contains = true;

                    }

                    foreach (KeyValuePair<Part, int> existingPart in existing)
                    {
                        
                        if (existingPart.Key.partID == result.partID) contains = true;
                        
                    }

                    if (!contains)
                    {
                        
                        parts.Add(result);
                        
                    }
                    
                }
                
            }
            
            ViewData["Parts"] = parts;
            ViewData["Search"] = search;
            ViewData["JobID"] = jobID;
            ViewData["Vehicle"] = JobProvider.getVehicleFromJob(jobID);

            return View("AddPart");

        }

        public IActionResult AssignPart(string jobID, string partID, string quantity)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            PartsProvider.assignPartToJob(jobID, partID, Int32.Parse(quantity));
            
            return RedirectToAction("ViewJob", new {id = jobID});

        }
        
        public IActionResult GetMOTReminders()
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }

            ViewData["Jobs"] = JobProvider.getMOTReminders();

            return View("ViewMOTReminders");


        }
        
        public IActionResult MOTReminder(string jobID)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }

            ViewData["Job"] = JobProvider.getJobDetails(jobID);
            
            return View("MOTReminder");


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
