using System.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using GARITS.Models;

namespace GARITS.Providers
{
    public static class PartsProvider
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
        
        
    }
}