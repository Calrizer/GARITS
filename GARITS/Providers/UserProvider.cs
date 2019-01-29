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

namespace GARITS.Providers
{
    public static class UserProvider
    {

        private static string connection = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        //Returns all users in the GARITS system.

        public static List<User> getUsers()
        {

            List<User> users = new List<User>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT id, name, email, role FROM Users";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            users.Add(new User
                            {

                                username = (sdr["username"]).ToString(),
                                name = sdr["name"].ToString(),
                                role = sdr["role"].ToString()


                            });

                        }

                    }

                    con.Close();

                }

            }

            return users;

        }


        public static User getUserFromUsername(string username)
        {

            User user = new User();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT id, name, email, role FROM Users WHERE username = @username";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            user = (new User
                            {

                                username = (sdr["username"]).ToString(),
                                name = sdr["name"].ToString(),
                                role = sdr["role"].ToString()


                            });

                            break;

                        }

                    }

                    con.Close();

                }

            }

            return user;

        }


        public static bool checkCredentials(string username, string password)
        {

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT password FROM Users WHERE username = @username";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            if (sdr["password"].ToString() == password)
                            {
                                con.Close();
                                return true;
                            }

                        }

                    }

                    con.Close();

                }

            }

            return false;

        }

        public static void setCurrentUser(User current)
        {



        }

    }

}
