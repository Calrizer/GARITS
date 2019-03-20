using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GARITS.Models;
using GARITS.Controllers;
using GARITS.Providers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GARITS.Controllers
{
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

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
            string[] lines = { "WRITE TO FILE", user.username, user.firstname, user.lastname, user.role, password };
            System.IO.File.WriteAllLines(@"C:\Users\Amber\Documents\Networks and Operating Systems\writetext.txt", lines);


            UserProvider.addUser(user, password);
            return RedirectToAction("ManageAccounts");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        /*public IActionResult ChangeName(User user)
        {
            string name = user.firstname;
            UserProvider.changeName(name, getAuthenticatedUser().username);
            return RedirectToAction("Index");
        }*/

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
