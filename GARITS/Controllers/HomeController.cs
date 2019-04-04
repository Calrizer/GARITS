
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GARITS.Models;
using GARITS.Controllers;
using GARITS.Providers;


namespace GARITS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }

            ViewData["User"] = getAuthenticatedUser();
            
            switch (getAuthenticatedUser().role)
            {
                
                case "admin":
                    return RedirectToAction("Dashboard", "admin");
                
                case "receptionist":
                    return View("Receptionist");
                
                case "franchisee":
                    return View("Franchisee");
                
                case "mechanic":
                    return View("Mechanic");
                
                case "foreperson":
                    return View("Foreperson");
                
            }

            return View();

        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
