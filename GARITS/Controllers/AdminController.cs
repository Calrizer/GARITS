using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            ViewData["User"] = getAuthenticatedUser();

            return View();

        }

        public IActionResult ManageAccounts()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(string username, string firstname, string lastname, string role, string password)
        {

            User user = new User
            {
                username = username,
                firstname = firstname,
                lastname = lastname,
                role = role
            };


            UserProvider.addUser(user, password);
            return RedirectToAction("ManageAccounts");
        }

        public IActionResult RemoveAccount(string username)
        {
            UserProvider.removeUser(username);
            return RedirectToAction("ManageAccounts");
        }

        public IActionResult EditAccount(string username, string firstname, string lastname, string role, float rate, string password)
        {
            UserProvider.editAccount(username, firstname, lastname, role, rate, password);
            return RedirectToAction("ManageAccounts");
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
            
            try
            {
                ViewData["DVLACar"] = VehicleProvider.getDVLADetails(vrm);
            }
            catch
            {
                ViewData["DVLACar"] = new DVLAData();
            }

            ViewData["vrm"] = vrm;

            return View();

        }


        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
