using System;
namespace GARITS.Models

{
    public class User
    {

        public User() { }

        public User(string username, string firstname, string lastname, string role) {
            this.username = username;
            this.firstname = firstname;
            this.lastname = lastname;
            this.role = role;
        }

        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string role { get; set; }
        public float rate { get; set; }

    }
}
