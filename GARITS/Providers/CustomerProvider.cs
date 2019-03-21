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

    }

}
