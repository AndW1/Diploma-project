using Encoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.Encoder
{
    public class Cryptographer
    {
        private string password;

        public Cryptographer Create(string pass)
        {
            this.password = pass;

            return this;
        }

        public string Encode(string target)
        {
            string encode = new Encryptor().Encode(target, password);

            return encode;
        }


        public string Decode(string target)
        {
            string decode = new Encryptor().Decode(target, password);

            return decode;
        }

        public UserApp ConvertUserData(UserApp userApp, string new_passwordHash)
        {
            string nameOld = Decode(userApp.FirstName);
            string lastnameOld = Decode(userApp.LastName);
            string emailOld = Decode(userApp.Email);

            this.password = new_passwordHash;

            string nameNew = Encode(nameOld);
            string lastNameNew = Encode(lastnameOld);
            string emailNew = Encode(emailOld);

            userApp.FirstName = nameNew;
            userApp.LastName = lastNameNew;
            userApp.Email = emailNew;

            return userApp;
        }

    }
}
