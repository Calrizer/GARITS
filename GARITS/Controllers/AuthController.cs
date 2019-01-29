using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GARITS.Providers;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

            Console.Out.WriteLine(username + " - " + password);

            if (UserProvider.checkCredentials(username, password))
            {
                Console.Out.WriteLine("======Login Success======");

                HttpContext.Session.SetString("user", username);

                return RedirectToAction("index", "home");

            }

            TempData["LoginError"] = true;
            return RedirectToAction("Login");

        }

    }
}
