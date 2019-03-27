using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using GARITS.Models;
using GARITS.Providers;

namespace GARITS.Controllers
{
    public class VehicleController : Controller
    {
        // GET
        public IActionResult AddVehicle(string vrm)
        {

            ViewData["DVLADetails"] = VehicleProvider.getDVLADetails(vrm);
            
            return View();
            
        }
        
        [HttpPost]
        public IActionResult RegisterVehicle(string vrm, string make, string model, string year, string colour, string serial, string chassis)
        {

            Vehicle vehicle = new Vehicle
            {

                vrm = vrm,
                make = make,
                model = model,
                year = year,
                chassis = chassis,
                colour = colour,
                engine = serial

            };
            
            VehicleProvider.addVehicle(vehicle);

            return RedirectToAction("AddWithVehicle", "Customer", new {vrm = vrm});


        }
        
    }
    
}