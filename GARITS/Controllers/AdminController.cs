using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using GARITS.Providers;
using GARITS.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GARITS.Controllers
{
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Dashboard()
        {

            if (!isAuthenticated())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (getAuthenticatedUser().role != "admin")
            {

                TempData["Page"] = "Admin Dashboard";

                return RedirectToAction("PermissionsError", "Auth");

            }

            return View();

        }

        public IActionResult Users()
        {

            ViewData["Users"] = UserProvider.getUsers();

            return View();

        }

        [HttpPost]
        public IActionResult Car(string vrm)
        {

            ViewData["Car"] = VehicleProvider.getVehicleFromVRM(vrm);
            ViewData["DVLACar"] = VehicleProvider.getDVLADetails(vrm);

            return View();

        }

        private bool isAuthenticated()
        {

            if (HttpContext.Session.GetString("user") != null)
            {

                return true;

            }

            return false;

        }

        private User getAuthenticatedUser()
        {

            return UserProvider.getUserFromUsername(HttpContext.Session.GetString("user"));

        }

    }
}
