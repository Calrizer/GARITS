using System;
using System.IO;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GARITS.Providers;
using GARITS.Models;
using MySql.Data.MySqlClient;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GARITS.Controllers
{
    public class AdminController : Controller
    {
        
        private static string connection = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        
        // GET: /<controller>/
        public IActionResult Dashboard()
        {

            if (!isAuthenticated())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (getAuthenticatedUser().role != "admin")
            {

                TempData["Page"] = "Admin Dashboard";

                return RedirectToAction("PermissionsError", "Auth");

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


            UserProvider.addUser(user, password);
            return RedirectToAction("ManageAccounts");
        }

        public IActionResult RemoveAccount(string username)
        {
            UserProvider.removeUser(username);
            return RedirectToAction("ManageAccounts");
        }

        public IActionResult EditAccount(string username, string firstname, string lastname, string role, float rate, string password)
        {
            UserProvider.editAccount(username, firstname, lastname, role, rate, password);
            return RedirectToAction("ManageAccounts");
        }

        public IActionResult Users()
        {

            ViewData["Users"] = UserProvider.getUsers();

            return View();

        }

        [HttpPost]
        public IActionResult Car(string vrm)
        {

            vrm = vrm.Replace(" ", "");

            ViewData["Car"] = VehicleProvider.getVehicleFromVRM(vrm);
            
            try
            {
                ViewData["DVLACar"] = VehicleProvider.getDVLADetails(vrm);
            }
            catch
            {
                ViewData["DVLACar"] = new DVLAData();
            }

            ViewData["vrm"] = vrm;

            return View();

        }

        public IActionResult ViewBackups(string message)
        {

            ViewData["Backups"] = getAllBackupFiles();
            ViewData["Message"] = message;

            return View("ViewBackups");

        }
        
        public FileResult DownloadBackup(string file)
        {

            return PhysicalFile(AppDomain.CurrentDomain.BaseDirectory + "backups/" + file, "application/force-download", file);
        }
        
        public IActionResult Backup()
        {

            string file = AppDomain.CurrentDomain.BaseDirectory + "backups/" + DateTime.Now.ToString("dd:MM:yyyy-hh:mm") + "-MANUAL-.sql";
            Console.Out.WriteLine(file);
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(file);
                        conn.Close();
                    }
                }
            }

            return RedirectToAction("ViewBackups", new {message = "BACKUP"});

        }

        public IActionResult RestoreBackup(string file)
        {
            
            string restore = AppDomain.CurrentDomain.BaseDirectory + "backups/" + file;
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ImportFromFile(restore);
                        conn.Close();
                    }
                }
            }

            return RedirectToAction("ViewBackups", new {message = "RESTORE"});

        }
        
        public IActionResult DeleteBackup(string file)
        {
            
            string delete = AppDomain.CurrentDomain.BaseDirectory + "backups/" + file;
            
            System.IO.File.Delete(delete);
            
            return RedirectToAction("ViewBackups", new {message = "DELETE"});

        }

        private FileInfo[] getAllBackupFiles()
        {
            
            DirectoryInfo d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "backups/");
            return d.GetFiles("*.sql");
            
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
