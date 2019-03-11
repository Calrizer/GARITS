using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GARITS.Providers;
using Microsoft.AspNetCore.Mvc;
using GARITS.Models;

using Microsoft.AspNetCore.Http;

namespace GARITS.Controllers
{
    public class AuthController : Controller
    {
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Login()
        {

            if (TempData["LoginError"] == null)
            {
                ViewData["LoginError"] = false;

            }
            else
            {
                ViewData["LoginError"] = TempData["LoginError"];
            }

            return View();
       
        }

        [HttpGet]
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");

        }

        [HttpPost]
        public IActionResult PostLogin(string username, string password)
        {

            if (UserProvider.checkCredentials(username, password))
            {
                Console.Out.WriteLine("======Login Success======");

                HttpContext.Session.SetString("user", username);

                if (UserProvider.getUserFromUsername(username).role == "admin")
                {
                    return RedirectToAction("Dashboard", "admin");
                } else {
                    return RedirectToAction("index", "home");
                }

            }

            TempData["LoginError"] = true;
            return RedirectToAction("Login");

        }


        [HttpGet]
        public IActionResult PermissionsError()
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }

            if (TempData["Page"] == null)
            {

                return RedirectToAction("index", "home");

            }

            ViewData["Page"] = TempData["Page"];

            return View();

        }

        public bool isAuthenticated()
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
