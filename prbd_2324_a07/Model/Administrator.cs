using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;


namespace prbd_2324_a07.Model;

    public class Administrator : User
    {
        protected Administrator(int id, string email, string hashed_password, string full_name) 
        : base(id, email, hashed_password, full_name) {
        Role = Role.Administrator;
    }
    public Administrator() {
        Role = Role.Administrator;
        }
        public override string ToString() {
        return $"Id: {Id}, Email: {Email}, Password: {Hashed_Password}, Full_Name : {Full_name}";
        }
}

