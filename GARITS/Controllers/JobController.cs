using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GARITS.Models;
using GARITS.Providers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GARITS.Controllers
{
    public class JobController : Controller
    {
        // GET: /<controller>/

        public IActionResult ViewJob(string id)
        {

            string jobID = id;

            ViewData["JobDetails"] = JobProvider.getJobDetails(jobID);

            return View("Job");

        }

        public IActionResult Book()
        {

            return View("Book");

        }

        [HttpPost]
        public IActionResult BookVehicle(string type, string issue, string cosmetic, string resolution, string labour, string bay, string customerID, string vrm, string mechanic)
        {

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
                start = DateTime.Today,
                end = null,
                paid = null,
                bay = Int32.Parse(bay),
                status = "Ready For Repair",
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

            Vehicle vehicle = VehicleProvider.getVehicleFromVRM(vrm);

            if (vehicle.vrm == null) return RedirectToAction("AddVehicle", "Vehicle", new {vrm = vrm});

            ViewData["Vehicle"] = vehicle;
            Customer customer = VehicleProvider.getCustomerFromVehicle(vrm);
            ViewData["Customer"] = customer;
            ViewData["Debt"] = CustomerProvider.checkIfInDebt(customer.customerID);
            ViewData["Mechanic"] = UserProvider.getUsers();

            return View("Book");

        }

        [HttpPost]
        public IActionResult Invoice(string jobID)
        {

            ViewData["InvoiceNote"] = JobProvider.getInvoiceNote(jobID);
            ViewData["JobDetails"] = JobProvider.getJobDetails(jobID);

            return View("Invoice");

        }

        [HttpPost]
        public IActionResult JobSheet(string jobID)
        {

            ViewData["EstimateNote"] = JobProvider.getEstimateNote(jobID);
            ViewData["JobDetails"] = JobProvider.getJobDetails(jobID);
            
            return View("JobSheet");

        }

        [HttpPost]
        public IActionResult AddNote(string type, string body, string jobID)
        {

            JobNote note = new JobNote
            {

                id = JobProvider.GetUniqueKey(255),
                type = type,
                body = body,
                time = DateTime.Now,
                user = UserProvider.getUserFromUsername(HttpContext.Session.GetString("user"))

            };

            JobProvider.addJobNote(note, jobID);

            TempData["JobID"] = jobID;

            return RedirectToAction("ViewJob", "Job", new { id = jobID });

        }

        [HttpPost]
        public IActionResult AddPart(string jobID, string search)
        {

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

            PartsProvider.assignPartToJob(jobID, partID, Int32.Parse(quantity));
            
            return RedirectToAction("ViewJob", new {id = jobID});

        }

    }

}
