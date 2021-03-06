﻿using System;
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
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Parts"] = PartsProvider.getParts();
            return View();
        }

        [HttpGet]
        public IActionResult AddNewPart()
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            return View();
        }

        [HttpPost]
        public IActionResult AddNewPart(string partID, string name, string manufacturer, string vehicle, string years, float price, int quantity, int threshold)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
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
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Part"] = PartsProvider.getPartFromID(partID);
            return View();
        }

        [HttpPost]
        public IActionResult ChangePartDetails(string partID, string name, string manufacturer, string vehicle, string years, float price, int quantity, int threshold)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
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
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["Parts"] = PartsProvider.getLowStockParts();
            return View();
        }

        [HttpPost]
        public IActionResult ReplenishStock(string partID, int quantity)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            PartsProvider.replenishStock(partID, quantity);
            return RedirectToAction("ReplenishStock");
        }

        [HttpPost]
        public IActionResult OrderAddCustomer(string customerID)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            Customer customer = CustomerProvider.getCustomerFromID(customerID);
            TempData ["customerID"] = customerID;
            if (customer.customerID == null) return RedirectToAction("OrderNewCustomer", "Part");

            ViewData["Customer"] = customer;

            PartsProvider.clearOrder(customer.customerID);
            PartsProvider.RemovePartsOrders(customer.customerID);
            Order order = new Order
            {
                orderID = customer.customerID,
                date = customer.registered,
                addressline1 = customer.addressline1,
                addressline2 = customer.addressline2,
                town = customer.addressline2,
                county = customer.county,
                postcode = customer.postcode,
                username = HttpContext.Session.GetString("user"),
            };
            PartsProvider.partsOrder(order);


            return RedirectToAction("OrderParts", new { customerID = customerID });
        }

        [HttpGet]
        public IActionResult OrderNewCustomer(string customerID)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["customerID"] = customerID;
            return View(customerID);
        }

        [HttpPost]
        public IActionResult OrderNewCustomer(string customerID, string email, DateTime registered, string title, string firstname, string lastname, string addressline1, string addressline2, string county, string postcode, string phone)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            Customer customer = new Customer
            {
                customerID = customerID,
                email = email,
                registered = registered,
                title = title,
                firstname = firstname,
                lastname = lastname,
                addressline1 = addressline1,
                addressline2 = addressline2,
                county = county,
                postcode = postcode,
                phone = phone
            };
            CustomerProvider.addCustomer(customer);

            Order order = new Order
            {
                orderID = customerID,
                date = registered,
                addressline1 = addressline1,
                addressline2 = addressline2,
                town = addressline2,
                county = county,
                postcode = postcode,
                username = HttpContext.Session.GetString("user"),
            };
            PartsProvider.partsOrder(order);

            ViewData["Customer"] = customer;
            return RedirectToAction("OrderParts", new { customerID = customerID });
        }
        
        [HttpGet]
        public IActionResult OrderParts(string customerID)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            ViewData["CustomerID"] = customerID;
            ViewData["Parts"] = PartsProvider.getParts();

            Dictionary<Part, int> cart = new Dictionary<Part, int>();
            TempData["ShoppingCart"] = cart;
            return View();
        }

        [HttpPost]
        public IActionResult AddPart(string partID, string customerID)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            Dictionary<Part, int> parts = PartsProvider.getOrder(customerID);
            bool exists = false;
            if (parts != null)
            {
                foreach (KeyValuePair<Part, int> part in parts)
                {
                    if (part.Key.partID == partID)
                    {
                        PartsProvider.PartOrderAddQuantity(customerID, partID, 1);
                        exists = true;
                    }
                }

            }
            if (!exists)
            {
                PartsProvider.AddPartToOrder(customerID, partID, 1);
                parts.Add(PartsProvider.getPartFromID(partID), 1);
            }
            TempData["ShoppingCart"] = parts;
            ViewData["CustomerID"] = customerID;
            ViewData["Parts"] = PartsProvider.getParts();
            return View("OrderParts");
        }
        public IActionResult GenerateInvoice(string customerID)
        {
            
            if (!isAuthenticated())
            {

                return RedirectToAction("Login", "Auth");

            }
            
            Dictionary<Part, int> parts = PartsProvider.getOrder(customerID);
            ViewData["Parts"] = parts;
            ViewData["Order"] = PartsProvider.getOrderDetails(customerID);
            PartsProvider.clearOrder(customerID);
            return View("PartInvoice");
        }
        
        private bool isAuthenticated()
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