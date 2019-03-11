using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GARITS.Providers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GARITS.Controllers
{
    public class JobController : Controller
    {
        // GET: /<controller>/

        public IActionResult ViewJob()
        {

            ViewData["Notes"] = JobProvider.getJobNotes("000001");
            ViewData["Customer"] = CustomerProvider.getCustomerFromEmail("csilva2@apple.com");
            ViewData["Vehicle"] = VehicleProvider.getVehicleFromVRM("Y881MJM");
            ViewData["Parts"] = JobProvider.getPartsForJob("000001");
            ViewData["Labour"] = JobProvider.getLabour("000001");
            ViewData["Mechanic"] = JobProvider.getAssigedMechanic("000001");

            return View("Job");

        }
    }
}
