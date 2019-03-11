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
    public static class JobProvider
    {

        private static string connection = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        public static List<JobNote> getJobNotes(string jobID)
        {

            List<JobNote> notes = new List<JobNote>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT notes.* FROM JobNotes AS notes, JobNotesJob AS job WHERE job.jobID = " + jobID + " AND notes.noteID = job.noteID AND NOT (notes.type = 'Labour Estimate' OR notes.type = 'Labour Actual') ORDER BY notes.time DESC";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            notes.Add(new JobNote
                            {

                                id = (sdr["noteID"]).ToString(),
                                body = sdr["body"].ToString(),
                                type = sdr["type"].ToString(),
                                time = DateTime.Parse(sdr["time"].ToString()),
                                user = UserProvider.getUserFromUsername(sdr["username"].ToString())


                            });

                        }

                    }

                    con.Close();

                }

            }

            return notes;

        }

        public static Dictionary<string, float> getLabour(string jobID)
        {

            Dictionary<string, float> labour = new Dictionary<string, float>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT notes.type, notes.body FROM JobNotes AS notes, JobNotesJob AS job WHERE job.jobID = " + jobID + " AND notes.noteID = job.noteID AND (notes.type = 'Labour Estimate' OR notes.type = 'Labour Actual')";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            labour.Add(sdr["type"].ToString(), float.Parse(sdr["body"].ToString()));

                        }

                    }

                    con.Close();

                }

            }

            return labour;

        }

        public static Dictionary<Part, int> getPartsForJob(string jobID)
        {

            Dictionary<Part, int> parts = new Dictionary<Part, int>();


            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT parts.*, job.quantity FROM Parts AS parts, PartsJobs AS job WHERE job.jobID = " + jobID + " AND parts.partID = job.partID";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            Part part = (new Part
                            {

                                partID = sdr["partID"].ToString(),
                                name = sdr["name"].ToString(),
                                manufacturer = sdr["manufacturer"].ToString(),
                                vehicle = sdr["vehicle"].ToString(),
                                years = sdr["years"].ToString(),
                                price = float.Parse(sdr["price"].ToString()),
                                quantity = Int32.Parse(sdr["stockquantity"].ToString()),
                                threshold = Int32.Parse(sdr["threshold"].ToString())

                            });

                            parts.Add(part, Int32.Parse(sdr["quantity"].ToString()));

                        }

                    }

                    con.Close();

                }

            }

            return parts;

        }

        public static User getAssigedMechanic(string jobID)
        {

            User user = new User();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT username FROM UserJob WHERE jobID = " + jobID;
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            user = UserProvider.getUserFromUsername(sdr["username"].ToString());

                        }

                    }

                    con.Close();

                }

            }

            return user;

        }

    }

}
