using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GARITS.Models;
using System.Data;
using System.Configuration;
using System.Globalization;
using MySql.Data.MySqlClient;
using GARITS.Providers;

namespace GARITS.Providers
{
    public class CustomerProvider
    {

        private static string connection = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        public static Customer getCustomerFromID(string customerID)
        {

            Customer customer = new Customer();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Customers WHERE customerID = '" + customerID + "'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            customer = (new Customer
                            {

                                customerID = (sdr["customerID"]).ToString(),
                                email = sdr["email"].ToString(),
                                registered = DateTime.Parse(sdr["registered"].ToString()),
                                title = sdr["title"].ToString(),
                                firstname = sdr["firstname"].ToString(),
                                lastname = sdr["lastname"].ToString(),
                                addressline1 = sdr["addressline1"].ToString(),
                                addressline2 = sdr["addressline2"].ToString(),
                                county = sdr["county"].ToString(),
                                postcode = sdr["postcode"].ToString(),
                                phone = sdr["tel"].ToString()

                            });

                            break;

                        }

                    }

                    con.Close();

                }

            }

            return customer;

        }

        public static List<Customer> getAllCustomers()
        {
            
            List<Customer> customers = new List<Customer>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Customers ORDER BY registered DESC";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            customers.Add(new Customer
                            {

                                customerID = (sdr["customerID"]).ToString(),
                                email = sdr["email"].ToString(),
                                registered = DateTime.Parse(sdr["registered"].ToString()),
                                title = sdr["title"].ToString(),
                                firstname = sdr["firstname"].ToString(),
                                lastname = sdr["lastname"].ToString(),
                                addressline1 = sdr["addressline1"].ToString(),
                                addressline2 = sdr["addressline2"].ToString(),
                                county = sdr["county"].ToString(),
                                postcode = sdr["postcode"].ToString(),
                                phone = sdr["tel"].ToString()

                            });
                            
                        }

                    }

                    con.Close();

                }
            }

            return customers;
            
        }
        
        public static List<Customer> searchCustomers(string search)
        {
            
            List<Customer> customers = new List<Customer>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Customers WHERE customerID LIKE '%" + search + "%' OR email LIKE '%" + search +
                               "%' OR firstname LIKE '%" + search + "%' OR lastname LIKE '%" + search +
                               "%' OR postcode LIKE '%" + search + "%' OR tel LIKE '%" + search + "%'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            customers.Add(new Customer
                            {

                                customerID = (sdr["customerID"]).ToString(),
                                email = sdr["email"].ToString(),
                                registered = DateTime.Parse(sdr["registered"].ToString()),
                                title = sdr["title"].ToString(),
                                firstname = sdr["firstname"].ToString(),
                                lastname = sdr["lastname"].ToString(),
                                addressline1 = sdr["addressline1"].ToString(),
                                addressline2 = sdr["addressline2"].ToString(),
                                county = sdr["county"].ToString(),
                                postcode = sdr["postcode"].ToString(),
                                phone = sdr["tel"].ToString()

                            });
                            
                        }

                    }

                    con.Close();

                }
            }

            return customers;
            
        }
        
        public static List<Vehicle> getCustomerVehicles(string customerID)
        {
            
            List<Vehicle> vehicles = new List<Vehicle>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT vrm FROM CustomersVehicles WHERE customerID = '" + customerID + "'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            vehicles.Add(VehicleProvider.getVehicleFromVRM(sdr["vrm"].ToString()));

                        }

                    }

                    con.Close();

                }
            }

            return vehicles;
            
        }
        
        public static List<Job> getCustomerJobs(string customerID)
        {
            
            List<Job> jobs = new List<Job>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT jv.jobID FROM CustomersVehicles as cv, Vehicles as v, JobVehicle as jv WHERE cv.customerID = '" + customerID + "' AND v.vrm = cv.vrm AND jv.vrm = v.vrm";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            jobs.Add(JobProvider.getJobDetails(sdr["jobID"].ToString()));

                        }

                    }

                    con.Close();

                }
            }

            return jobs;
            
        }
        
        public static void addCustomer(Customer customer)
        {
            
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO Customers VALUES (@customerID, @email, @registered, @title, @firstname, @lastname, @addressLine1, @addressLine2, @county, @postcode, @tel)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@customerID", customer.customerID);
                    cmd.Parameters.AddWithValue("@email", customer.email);
                    cmd.Parameters.AddWithValue("@registered", customer.registered);
                    cmd.Parameters.AddWithValue("@title", customer.title);
                    cmd.Parameters.AddWithValue("@firstname", customer.firstname);
                    cmd.Parameters.AddWithValue("@lastname", customer.lastname);
                    cmd.Parameters.AddWithValue("@addressLine1", customer.addressline1);
                    cmd.Parameters.AddWithValue("@addressLine2", customer.addressline2);
                    cmd.Parameters.AddWithValue("@county", customer.county);
                    cmd.Parameters.AddWithValue("@postcode", customer.postcode);
                    cmd.Parameters.AddWithValue("@tel", customer.phone);
                    
                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }
            
        }
        
        public static void updateCustomer(Customer customer)
        {
            
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "UPDATE Customers SET email = @email, title = @title, firstname = @firstname, lastname = @lastname, addressLine1 = @addressLine1, addressLine2 = @addressLine2, county = @county, postcode = @postcode, tel = @tel WHERE customerID = @customerID";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@customerID", customer.customerID);
                    cmd.Parameters.AddWithValue("@email", customer.email);
                    cmd.Parameters.AddWithValue("@title", customer.title);
                    cmd.Parameters.AddWithValue("@firstname", customer.firstname);
                    cmd.Parameters.AddWithValue("@lastname", customer.lastname);
                    cmd.Parameters.AddWithValue("@addressLine1", customer.addressline1);
                    cmd.Parameters.AddWithValue("@addressLine2", customer.addressline2);
                    cmd.Parameters.AddWithValue("@county", customer.county);
                    cmd.Parameters.AddWithValue("@postcode", customer.postcode);
                    cmd.Parameters.AddWithValue("@tel", customer.phone);
                    
                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }
            
        }

        public static void assignVehicle(string vrm, string customerID)
        {
            
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO CustomersVehicles VALUES (@vrm, @customerID)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@vrm", vrm);
                    cmd.Parameters.AddWithValue("@customerID", customerID);
                    
                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }
            
        }

        public static string checkIfInDebt(string customerID)
        {

            string debt = "No";

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT j.jobID FROM CustomersVehicles as cv, Vehicles as v, JobVehicle as jv, Jobs as j WHERE cv.customerID = '" + customerID + "' AND v.vrm = cv.vrm AND jv.vrm = v.vrm AND j.jobID = jv.jobID AND j.status = 'Complete - Awaiting Payment'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            debt = sdr["jobID"].ToString();

                        }

                    }

                    con.Close();

                }

            }

            return debt;

        }

        public static Discount getDiscounts(string customerID)
        {
            
            Discount discount = new Discount();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Discounts WHERE customerID = '" + customerID + "'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            discount.customer = getCustomerFromID(sdr["customerID"].ToString());
                            discount.mot = Int32.Parse(sdr["mot"].ToString());
                            discount.parts = Int32.Parse(sdr["parts"].ToString());
                            discount.other = Int32.Parse(sdr["other"].ToString());
                            discount.credit = float.Parse(sdr["credit"].ToString());

                        }

                    }

                    con.Close();

                }

            }

            return discount;
            
        }

    }

}
