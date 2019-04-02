using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using GARITS.Models;
using GARITS.Providers;

namespace GARITS.Controllers
{
    public class VehicleController : Controller
    {
        // GET
        public IActionResult ViewVehicles()
        {

            ViewData["Vehicles"] = VehicleProvider.getAllVehicles();

            return View("View");

        }

        public IActionResult Search(string search)
        {
            
            ViewData["Vehicles"] = VehicleProvider.searchVehicles(search);
            ViewData["Search"] = search;

            return View("View");
            
            
        }

        public IActionResult ViewVehiclesJobs(string vrm)
        {

            ViewData["Vehicle"] = VehicleProvider.getVehicleFromVRM(vrm);
            
            List<Job> jobs = new List<Job>();

            foreach (string jobID in JobProvider.getPreviousJobs(vrm, ""))
            {
                jobs.Add(JobProvider.getJobDetails(jobID));
            }

            ViewData["Jobs"] = jobs;

            return View("ViewJobs");

        }

        public IActionResult AddVehicle(string vrm)
        {

            vrm = vrm.Replace(" ", "");

            try
            {
                ViewData["DVLADetails"] = VehicleProvider.getDVLADetails(vrm);
            }
            catch
            {
                ViewData["DVLADetails"] = new DVLAData();
            }

            ViewData["vrm"] = vrm;

            return View();
            
        }

        public IActionResult EditVehicle(string vrm)
        {

            ViewData["Vehicle"] = VehicleProvider.getVehicleFromVRM(vrm);

            return View("EditVehicle");

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
        
        [HttpPost]
        public IActionResult UpdateVehicle(string vrm, string make, string model, string year, string colour, string serial, string chassis)
        {

            Vehicle vehicle = VehicleProvider.getVehicleFromVRM(vrm);

            vehicle.vrm = vrm;
            vehicle.make = make;
            vehicle.model = model;
            vehicle.year = year;
            vehicle.colour = colour;
            vehicle.engine = serial;
            vehicle.chassis = chassis;
            
            VehicleProvider.updateVehicle(vehicle);

            return RedirectToAction("Search", new {search = vrm});

        }
        
    }
    
}