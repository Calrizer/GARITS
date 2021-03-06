﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

using GARITS.Models;


namespace GARITS.Providers
{
    public static class VehicleProvider
    {

        private static string connection = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        public static DVLAData getDVLADetails(string vrm)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://moneysupermarket-prod.apigee.net/gb/reference-data/v0/vehicles?registrationNumber=" + vrm);

            request.Headers.Add("client_id", "EHtEf346HoQnp54Rj7RuGbg1eGAuAwIN");
            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            StreamReader reader = new StreamReader(response.GetResponseStream());

            string json = reader.ReadToEnd();

            json.TrimStart(new char[] { '{' }).TrimEnd(new char[] { '}' });

            JObject data = JObject.Parse(json);
            JObject cardata = JObject.Parse(data["vehicles"][0].ToString());

            string fuel;

            switch (cardata["engineTypeId"].ToString())
            {
                case "1":
                    fuel = "Petrol";
                    break;

                case "2":
                    fuel = "Diesel";
                    break;

                case "3":
                    fuel = "Electric";
                    break;

                default:
                    fuel = "Unknown";
                    break;
            }

            DVLAData car = new DVLAData
            {
                vrm = vrm,
                make = cardata["makeName"].ToString(),
                model = cardata["modelName"].ToString(),
                varient = cardata["variantName"].ToString(),
                year = Convert.ToInt32(cardata["manufacturedYear"].ToString()),
                fuel = fuel
            };

            return car;

        }


        public static List<Vehicle> getAllVehicles()
        {

            List<Vehicle> vehicles = new List<Vehicle>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Vehicles";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            vehicles.Add(new Vehicle
                            {

                                vrm = (sdr["vrm"]).ToString(),
                                make = sdr["make"].ToString(),
                                model = sdr["model"].ToString(),
                                year = sdr["year"].ToString(),
                                engine = sdr["serial"].ToString(),
                                chassis = sdr["chassis"].ToString(),
                                colour = sdr["colour"].ToString()

                            });

                        }

                    }

                    con.Close();

                }

            }

            return vehicles;

        }
        
        public static List<Vehicle> searchVehicles(string search)
        {

            List<Vehicle> vehicles = new List<Vehicle>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Vehicles WHERE vrm LIKE '%" + search + "%' OR make LIKE '%" + search + "%' OR model LIKE '%" + search + "%' OR year LIKE '%" + search + "%' OR serial LIKE '%" + search + "%' OR chassis LIKE '%" + search + "%'" ;
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            vehicles.Add(new Vehicle
                            {

                                vrm = (sdr["vrm"]).ToString(),
                                make = sdr["make"].ToString(),
                                model = sdr["model"].ToString(),
                                year = sdr["year"].ToString(),
                                engine = sdr["serial"].ToString(),
                                chassis = sdr["chassis"].ToString(),
                                colour = sdr["colour"].ToString()

                            });

                        }

                    }

                    con.Close();

                }

            }

            return vehicles;

        }
        
        public static Vehicle getVehicleFromVRM(string vrm)
        {

            Vehicle car = new Vehicle();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Vehicles WHERE vrm = @vrm";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@vrm", vrm);
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            car = (new Vehicle
                            {

                                vrm = (sdr["vrm"]).ToString(),
                                make = sdr["make"].ToString(),
                                model = sdr["model"].ToString(),
                                year = sdr["year"].ToString(),
                                engine = sdr["serial"].ToString(),
                                chassis = sdr["chassis"].ToString(),
                                colour = sdr["colour"].ToString()

                            });

                        }

                    }

                    con.Close();

                }

            }

            return car;

        }

        public static Customer getCustomerFromVehicle(string vrm)
        {

            Customer customer = new Customer();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT customerID FROM CustomersVehicles WHERE vrm = @vrm";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@vrm", vrm);
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            customer = CustomerProvider.getCustomerFromID(sdr["customerID"].ToString());
                            break;

                        }

                    }

                    con.Close();

                }

            }

            return customer;

        }

        public static void addVehicle(Vehicle vehicle)
        {
            
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO Vehicles VALUES (@vrm, @make, @model, @year, @serial, @chassis, @colour)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@vrm", vehicle.vrm);
                    cmd.Parameters.AddWithValue("@make", vehicle.make);
                    cmd.Parameters.AddWithValue("@model", vehicle.model);
                    cmd.Parameters.AddWithValue("@year", vehicle.year);
                    cmd.Parameters.AddWithValue("@serial", vehicle.engine);
                    cmd.Parameters.AddWithValue("@chassis", vehicle.chassis);
                    cmd.Parameters.AddWithValue("@colour", vehicle.colour);
                    
                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }
            
        }
        
        public static void updateVehicle(Vehicle vehicle)
        {
            
            
            Console.Out.WriteLine(vehicle.vrm);
            
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "UPDATE Vehicles SET make = @make, model = @model, year = @year, serial = @serial, chassis = @chassis, colour = @colour WHERE vrm = @vrm";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@vrm", vehicle.vrm);
                    cmd.Parameters.AddWithValue("@make", vehicle.make);
                    cmd.Parameters.AddWithValue("@model", vehicle.model);
                    cmd.Parameters.AddWithValue("@year", vehicle.year);
                    cmd.Parameters.AddWithValue("@serial", vehicle.engine);
                    cmd.Parameters.AddWithValue("@chassis", vehicle.chassis);
                    cmd.Parameters.AddWithValue("@colour", vehicle.colour);
                    
                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }
            
        }

    }

}
