using System;
using GARITS.Models;
using GARITS.Providers;
using Microsoft.AspNetCore.Mvc;

namespace GARITS.Controllers
{
    public class CustomerController : Controller
    {
        // GET
        public IActionResult AddWithVehicle(string vrm)
        {

            ViewData["Vehicle"] = VehicleProvider.getVehicleFromVRM(vrm);
            
            return View("AddCustomerWithVehicle");
            
        }
        
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
    }
}