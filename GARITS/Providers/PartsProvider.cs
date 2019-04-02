using System;
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
    public class PartsProvider
    {

        private static string connection = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        
        public static List<Part> searchForParts(string search)
        {
            
            List<Part> parts = new List<Part>();
            
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Parts WHERE partID LIKE '%" + search + "%' OR name LIKE '%" + search + "%' OR manufacturer LIKE '%" + search + "%' OR vehicle LIKE '%" + search + "%'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            parts.Add(new Part
                            {
                                
                                partID = sdr["partID"].ToString(),
                                name = sdr["name"].ToString(),
                                manufacturer = sdr["manufacturer"].ToString(),
                                vehicle = sdr["vehicle"].ToString(),
                                years = sdr["years"].ToString(),
                                price = float.Parse(sdr["price"].ToString()),
                                quantity = Int32.Parse(sdr["stockQuantity"].ToString()),
                                threshold = Int32.Parse(sdr["threshold"].ToString())
                                
                            });

                        }

                    }

                    con.Close();

                }

            }

            return parts; 
            
        }

        public static void assignPartToJob(string jobID, string partID, int quantity)
        {
            
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO PartsJobs VALUES (@partID, @jobID, @quantity)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@partID", partID);
                    cmd.Parameters.AddWithValue("@jobID", jobID);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    
                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }
            
        }
        
        public static List<Part> getParts()
        {
            List<Part> parts = new List<Part>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Parts";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();

                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            parts.Add(new Part
                            {

                                partID = (sdr["partID"]).ToString(),
                                name = (sdr["name"]).ToString(),
                                manufacturer = (sdr["manufacturer"]).ToString(),
                                vehicle = (sdr["vehicle"]).ToString(),
                                years = (sdr["years"]).ToString(),
                                price = float.Parse((sdr["price"]).ToString()),
                                quantity = int.Parse((sdr["stockquantity"]).ToString()),
                                threshold = int.Parse((sdr["threshold"]).ToString())


                            });
                        }
                    }
                    con.Close();

                    return parts;
                }
            }
        }

        public static Part getPartFromID(string partID)
        {
            Part part = new Part() ;
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                
                string query = "SELECT * FROM Parts WHERE partID = @partID";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@partID", partID);

                    cmd.Connection = con;
                    con.Open();

                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            part = (new Part
                            {

                                partID = (sdr["partID"]).ToString(),
                                name = (sdr["name"]).ToString(),
                                manufacturer = (sdr["manufacturer"]).ToString(),
                                vehicle = (sdr["vehicle"]).ToString(),
                                years = (sdr["years"]).ToString(),
                                price = float.Parse((sdr["price"]).ToString()),
                                quantity = int.Parse((sdr["stockquantity"]).ToString()),
                                threshold = int.Parse((sdr["threshold"]).ToString())


                            });

                            break;

                        }

                    }

                    con.Close();
                }
            }
            return part;

        }

                public static void addNewPart(Part part)
        {
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO Parts VALUES (@partID, @name, @manufacturer, @vehicle, @years, @price, @quantity, @threshold)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@partID", part.partID);
                    cmd.Parameters.AddWithValue("@name", part.name);
                    cmd.Parameters.AddWithValue("@manufacturer", part.manufacturer);
                    cmd.Parameters.AddWithValue("@vehicle", part.vehicle);
                    cmd.Parameters.AddWithValue("@years", part.years);
                    cmd.Parameters.AddWithValue("@price", part.price);
                    cmd.Parameters.AddWithValue("@quantity", part.quantity);
                    cmd.Parameters.AddWithValue("@threshold", part.threshold);


                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }

        public static void replenishStock(string partID, int quantity)
        {
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "UPDATE Parts SET stockquantity = (stockquantity + @quantity) WHERE partID = @partID";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@partID", partID);
                    cmd.Parameters.AddWithValue("@quantity", quantity);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }
            }
        }

        public static void editPartDetails(Part part)
        {
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "UPDATE Parts SET name = @name, manufacturer = @manufacturer, vehicle = @vehicle, years = @years, price = @price, stockquantity = @quantity, threshold = @threshold  WHERE partID = @partID";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@partID", part.partID);
                    cmd.Parameters.AddWithValue("@name", part.name);
                    cmd.Parameters.AddWithValue("@manufacturer", part.manufacturer);
                    cmd.Parameters.AddWithValue("@vehicle", part.vehicle);
                    cmd.Parameters.AddWithValue("@years", part.years);
                    cmd.Parameters.AddWithValue("@price", part.price);
                    cmd.Parameters.AddWithValue("@quantity", part.quantity);
                    cmd.Parameters.AddWithValue("@threshold", part.threshold);


                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }

        public static List<Part> getLowStockParts()
        {
            List<Part> parts = new List<Part>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Parts WHERE stockquantity <= threshold";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();

                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            parts.Add(new Part
                            {

                                partID = (sdr["partID"]).ToString(),
                                name = (sdr["name"]).ToString(),
                                manufacturer = (sdr["manufacturer"]).ToString(),
                                vehicle = (sdr["vehicle"]).ToString(),
                                years = (sdr["years"]).ToString(),
                                price = float.Parse((sdr["price"]).ToString()),
                                quantity = int.Parse((sdr["stockquantity"]).ToString()),
                                threshold = int.Parse((sdr["threshold"]).ToString())


                            });
                        }
                    }
                    con.Close();

                    return parts;
                }
            }
        }
    }
}