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
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using GARITS.Providers;
namespace GARITS.Providers
{
    public static class JobProvider
    {

        private static string connection = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        public static Job getJobDetails(string jobID)
        {

            Job job = new Job();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT * FROM Jobs WHERE jobID = '" + jobID + "'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            job = new Job();

                            job.jobID = (sdr["jobID"]).ToString();
                            job.start = DateTime.Parse(sdr["startDate"].ToString());

                            try
                            {
                                job.end = DateTime.Parse(sdr["endDate"].ToString());
                            }
                            catch
                            {
                                job.end = null;
                            }

                            try
                            {
                                job.paid = DateTime.Parse(sdr["paidDate"].ToString());
                            }
                            catch
                            {
                                job.paid = null;
                            }

                            job.bay = Int32.Parse(sdr["bay"].ToString());
                            job.status = sdr["status"].ToString();
                            job.type = sdr["type"].ToString();
                            job.customer = VehicleProvider.getCustomerFromVehicle(getVehicleFromJob(jobID).vrm);
                            job.vehicle = getVehicleFromJob(jobID);
                            job.notes = getJobNotes(jobID);
                            job.parts = getPartsForJob(jobID);
                            job.labour = getLabour(jobID);
                            job.mechanic = getAssigedMechanic(jobID);

                          
                        }

                    }

                    con.Close();

                }

            }

            return job;

        }

        public static void createJob(Job job)
        {

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO Jobs (jobID, startDate, status, type, bay) VALUES (@jobID, @startDate, @status, @type, @bay)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@jobID", job.jobID);
                    cmd.Parameters.AddWithValue("@startDate", job.start);
                    cmd.Parameters.AddWithValue("@status", job.status);
                    cmd.Parameters.AddWithValue("@type", job.type);
                    cmd.Parameters.AddWithValue("@bay", job.bay);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

                foreach (JobNote note in job.notes)
                {

                    addJobNote(note, job.jobID);

                }

                assignVehicle(job.vehicle.vrm, job.jobID);
                assignMechanic(job.mechanic.username, job.jobID);

            }

        }

        public static Vehicle getVehicleFromJob(string jobID)
        {

            Vehicle vehicle = new Vehicle();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT vrm FROM JobVehicle WHERE jobID = '" + jobID + "'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            vehicle = VehicleProvider.getVehicleFromVRM(sdr["vrm"].ToString());
                            break;

                        }

                    }

                    con.Close();

                }

            }

            return vehicle;

        }

        public static void assignVehicle(string vrm, string jobID)
        {

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO JobVehicle VALUES (@jobID, @vrm)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@jobID", jobID);
                    cmd.Parameters.AddWithValue("@vrm", vrm);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }

        }

        public static List<JobNote> getJobNotes(string jobID)
        {

            List<JobNote> notes = new List<JobNote>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT notes.* FROM JobNotes AS notes, JobNotesJob AS job WHERE job.jobID = '" + jobID + "' AND notes.noteID = job.noteID AND NOT (notes.type = 'Labour Estimate' OR notes.type = 'Labour Actual') ORDER BY notes.time DESC";
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

        public static JobNote getInvoiceNote(string jobID)
        {

            JobNote note = new JobNote();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT notes.* FROM JobNotes AS notes, JobNotesJob AS job WHERE job.jobID = '" + jobID + "' AND notes.noteID = job.noteID AND notes.type = 'Invoice'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            note = new JobNote
                            {

                                id = (sdr["noteID"]).ToString(),
                                body = sdr["body"].ToString(),
                                type = sdr["type"].ToString(),
                                time = DateTime.Parse(sdr["time"].ToString()),
                                user = UserProvider.getUserFromUsername(sdr["username"].ToString())


                            };

                        }

                    }

                    con.Close();

                }

            }

            return note;


        }

        public static Dictionary<string, float> getLabour(string jobID)
        {

            Dictionary<string, float> labour = new Dictionary<string, float>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT notes.type, notes.body FROM JobNotes AS notes, JobNotesJob AS job WHERE job.jobID = '" + jobID + "' AND notes.noteID = job.noteID AND (notes.type = 'Labour Estimate' OR notes.type = 'Labour Actual')";
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
                string query = "SELECT parts.*, job.quantity FROM Parts AS parts, PartsJobs AS job WHERE job.jobID = '" + jobID + "' AND parts.partID = job.partID";
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
                string query = "SELECT username FROM UserJob WHERE jobID = '" + jobID + "'";
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

        public static void assignMechanic(string username, string jobID)
        {

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO UserJob VALUES (@username, @jobID)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@jobID", jobID);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }

        }

        public static void addJobNote(JobNote note, string jobID)
        {

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO JobNotes VALUES (@noteID, @body, @type, @time, @username)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@noteID", note.id);
                    cmd.Parameters.AddWithValue("@type", note.type);
                    cmd.Parameters.AddWithValue("@body", note.body);
                    cmd.Parameters.AddWithValue("@time", note.time);
                    cmd.Parameters.AddWithValue("@username", note.user.username);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

                query = "INSERT INTO JobNotesJob VALUES (@noteID, @jobID)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {

                    cmd.Parameters.AddWithValue("@noteID", note.id);
                    cmd.Parameters.AddWithValue("@jobID", jobID);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }

        }

        public static string GetUniqueKey(int size)
        {
            char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GetUniqueNumber(int size)
        {
            char[] chars =
                "1234567890".ToCharArray();
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

    }

}
