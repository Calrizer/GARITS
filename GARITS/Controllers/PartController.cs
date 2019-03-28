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
            ViewData["Parts"] = PartProvider.getParts();
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
            PartProvider.addNewPart(part);

            return RedirectToAction("Parts");

        }

        [HttpGet]
        public IActionResult ChangePartDetails(string partID)
        {
            ViewData["Part"] = PartProvider.getPartFromID(partID);
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
            PartProvider.editPartDetails(part);
            return RedirectToAction("Parts");
        }

        [HttpGet]
        public IActionResult ReplenishStock()
        {
            ViewData["Parts"] = PartProvider.getLowStockParts();
            return View();
        }

        [HttpPost]
        public IActionResult ReplenishStock(string partID, int quantity)
        {
            PartProvider.replenishStock(partID, quantity);
            return RedirectToAction("ReplenishStock");
        }
    }
}