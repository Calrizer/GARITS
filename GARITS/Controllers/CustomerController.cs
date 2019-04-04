using System;
using GARITS.Models;
using GARITS.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GARITS.Controllers
{
    public class CustomerController : Controller
    {
        // GET

        public IActionResult ViewCustomers()
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Customers"] = CustomerProvider.getAllCustomers();

            return View("View");

        }
        
        public IActionResult Search(string search)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Customers"] = CustomerProvider.searchCustomers(search);
            ViewData["Search"] = search;

            return View("View");

        }

        public IActionResult AddVehicleSearch(string vrm, string search)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Customers"] = CustomerProvider.searchCustomers(search);
            ViewData["Search"] = search;
            ViewData["Vehicle"] = VehicleProvider.getVehicleFromVRM(vrm);

            return View("AddVehicleSearchCustomer");

        }
        
        public IActionResult ViewCustomersVehicles(string customerID)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Customer"] = CustomerProvider.getCustomerFromID(customerID);
            ViewData["Vehicles"] = CustomerProvider.getCustomerVehicles(customerID);

            return View("ViewVehicles");

        }
        
        public IActionResult ViewCustomersJobs(string customerID)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Customer"] = CustomerProvider.getCustomerFromID(customerID);
            ViewData["Jobs"] = CustomerProvider.getCustomerJobs(customerID);

            return View("ViewJobs");

        }

        public IActionResult AddCustomer()
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            return View("AddCustomer");

        }
        
        
        public IActionResult AddWithVehicle(string vrm)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Vehicle"] = VehicleProvider.getVehicleFromVRM(vrm);
            
            return View("AddCustomerWithVehicle");
            
        }
        
        [HttpPost]
        public IActionResult RegisterCustomer(string title, string firstname, string lastname, string email, string tel, string address1, string address2, string county, string postcode)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            Customer customer = new Customer
            {

                customerID = JobProvider.GetUniqueKey(8),
                email = email,
                registered = DateTime.Today,
                title = title,
                firstname = firstname,
                lastname = lastname,
                addressline1 = address1,
                addressline2 = address2,
                county = county,
                postcode = postcode,
                phone = tel
               

            };
            
            CustomerProvider.addCustomer(customer);
            
            return RedirectToAction("ViewCustomers");
            
        }
        
        
        
        [HttpPost]
        public IActionResult RegisterCustomerWithVehicle(string vrm, string title, string firstname, string lastname, string email, string tel, string address1, string address2, string county, string postcode)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            Customer customer = new Customer
            {

                customerID = JobProvider.GetUniqueKey(8),
                email = email,
                registered = DateTime.Today,
                title = title,
                firstname = firstname,
                lastname = lastname,
                addressline1 = address1,
                addressline2 = address2,
                county = county,
                postcode = postcode,
                phone = tel
               

            };
            
            CustomerProvider.addCustomer(customer);
            CustomerProvider.assignVehicle(vrm, customer.customerID);
            
            return RedirectToAction("BookAddVehicle", "Job", new {vrm = vrm});
            
        }
        
        public IActionResult RegisterExistingCustomerWithVehicle(string vrm, string customerID)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            CustomerProvider.assignVehicle(vrm, customerID);
            
            return RedirectToAction("BookAddVehicle", "Job", new {vrm = vrm});
            
        }

        public IActionResult EditCustomer(string customerID)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Customer"] = CustomerProvider.getCustomerFromID(customerID);

            return View("EditCustomer");

        }
        
        [HttpPost]
        public IActionResult UpdateCustomer(string customerID, string title, string firstname, string lastname, string email, string tel, string address1, string address2, string county, string postcode)
        {

            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            Customer customer = CustomerProvider.getCustomerFromID(customerID);

            customer.title = title;
            customer.firstname = firstname;
            customer.lastname = lastname;
            customer.email = email;
            customer.phone = tel;
            customer.addressline1 = address1;
            customer.addressline2 = address2;
            customer.county = county;
            customer.postcode = postcode;
            
            CustomerProvider.updateCustomer(customer);
            
            return RedirectToAction("Search", new {search = customerID});
            
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