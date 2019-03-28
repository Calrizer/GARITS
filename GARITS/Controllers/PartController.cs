using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GARITS.Models;
using GARITS.Providers;

namespace GARITS.Controllers
{
    public class PartController : Controller
    {
        public IActionResult Parts()
        {
            ViewData["Parts"] = PartsProvider.getParts();
            return View();
        }

        [HttpGet]
        public IActionResult AddNewPart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddNewPart(string partID, string name, string manufacturer, string vehicle, string years, float price, int quantity, int threshold)
        {
            Part part = new Part
            {
                partID = partID,
                name = name,
                manufacturer = manufacturer,
                vehicle = vehicle,
                years = years,
                price = price,
                quantity = quantity,
                threshold = threshold
            };
            PartsProvider.addNewPart(part);

            return RedirectToAction("Parts");

        }

        [HttpGet]
        public IActionResult ChangePartDetails(string partID)
        {
            ViewData["Part"] = PartsProvider.getPartFromID(partID);
            return View();
        }

        [HttpPost]
        public IActionResult ChangePartDetails(string partID, string name, string manufacturer, string vehicle, string years, float price, int quantity, int threshold)
        {
            Part part = new Part
            {
                partID = partID,
                name = name,
                manufacturer = manufacturer,
                vehicle = vehicle,
                years = years,
                price = price,
                quantity = quantity,
                threshold = threshold
            };
            PartsProvider.editPartDetails(part);
            return RedirectToAction("Parts");
        }

        [HttpGet]
        public IActionResult ReplenishStock()
        {
            ViewData["Parts"] = PartsProvider.getLowStockParts();
            return View();
        }

        [HttpPost]
        public IActionResult ReplenishStock(string partID, int quantity)
        {
            PartsProvider.replenishStock(partID, quantity);
            return RedirectToAction("ReplenishStock");
        }
    }
}