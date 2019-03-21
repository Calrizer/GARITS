using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections;
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
                string query = "SELECT username, firstname, lastname, role FROM Users";
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
                                firstname = sdr["firstname"].ToString(),
                                lastname = sdr["lastname"].ToString(),
                                role = sdr["role"].ToString()


                            });

                        }

                    }

                    con.Close();

                }

            }

            return users;

        }

        //Returns User from a given username.

        public static User getUserFromUsername(string username)
        {

            User user = new User();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT username, firstname, lastname, role FROM Users WHERE username = @username";
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
                                firstname = sdr["firstname"].ToString(),
                                lastname = sdr["lastname"].ToString(),
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

        public static List<string> getPasswords()
        {
            List<string> passwords = new List<string>();

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "SELECT password FROM Users ORDER BY username";

                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read()) {
                            passwords.Add(sdr["password"].ToString());
                            break;
                        }
                    }
                    con.Close();
                }
            }

            return passwords;

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

        public static void addUser(User newUser, string password)
        {

            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "INSERT INTO users VALUES (@username, @password, @firstname, @lastname, @role)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@username", newUser.username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@firstname", newUser.firstname);
                    cmd.Parameters.AddWithValue("@lastname", newUser.lastname);
                    cmd.Parameters.AddWithValue("@role", newUser.role);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }

        public static void removeUser(string username)
        {
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "DELETE FROM users WHERE username = @username";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }

        public static void editAccount(string username, string firstname, string lastname, string role, string password)
        {
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "UPDATE users SET username = @username, firstname = @firstname, lastname = @lastname, role = @role WHERE username = @username";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@firstname", firstname);
                    cmd.Parameters.AddWithValue("@lastname", lastname);
                    cmd.Parameters.AddWithValue("@lastname", username);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }


        /*public static void changeName(string newName, string username)
        {
            using (MySqlConnection con = new MySqlConnection(connection))
            {
                string query = "UPDATE users SET firstname = @newName WHERE username = @username";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@newName", newName);
                    cmd.Parameters.AddWithValue("@username", username);

                    cmd.Connection = con;
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
            
        }*/

    }

}
