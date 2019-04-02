using System;
using GARITS.Models;
using GARITS.Providers;
using Microsoft.AspNetCore.Mvc;

namespace GARITS.Controllers
{
    public class CustomerController : Controller
    {
        // GET

        public IActionResult ViewCustomers()
        {

            ViewData["Customers"] = CustomerProvider.getAllCustomers();

            return View("View");

        }
        
        public IActionResult Search(string search)
        {

            ViewData["Customers"] = CustomerProvider.searchCustomers(search);
            ViewData["Search"] = search;

            return View("View");

        }
        
        public IActionResult ViewCustomersVehicles(string customerID)
        {

            ViewData["Customer"] = CustomerProvider.getCustomerFromID(customerID);
            ViewData["Vehicles"] = CustomerProvider.getCustomerVehicles(customerID);

            return View("ViewVehicles");

        }
        
        public IActionResult ViewCustomersJobs(string customerID)
        {

            ViewData["Customer"] = CustomerProvider.getCustomerFromID(customerID);
            ViewData["Jobs"] = CustomerProvider.getCustomerJobs(customerID);

            return View("ViewJobs");

        }

        public IActionResult AddCustomer()
        {

            return View("AddCustomer");

        }
        
        
        public IActionResult AddWithVehicle(string vrm)
        {

            ViewData["Vehicle"] = VehicleProvider.getVehicleFromVRM(vrm);
            
            return View("AddCustomerWithVehicle");
            
        }
        
        [HttpPost]
        public IActionResult RegisterCustomer(string title, string firstname, string lastname, string email, string tel, string address1, string address2, string county, string postcode)
        {

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

        public IActionResult EditCustomer(string customerID)
        {

            ViewData["Customer"] = CustomerProvider.getCustomerFromID(customerID);

            return View("EditCustomer");

        }
        
        [HttpPost]
        public IActionResult UpdateCustomer(string customerID, string title, string firstname, string lastname, string email, string tel, string address1, string address2, string county, string postcode)
        {

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
        
        
    }
}