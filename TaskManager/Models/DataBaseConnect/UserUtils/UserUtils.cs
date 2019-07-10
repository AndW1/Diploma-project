using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.DataBaseConnect.UserUtils
{
    public class UserUtils : IUserQuery
    {
        public UserApp GetUser(ApplicationContext applicationContext, string email, string password)
        {
            try
            {

                var user =applicationContext.UserApps.Where(u => u.Email == email && u.Upassword == password)
                            .Select(u => u).FirstOrDefault();
                return user;
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }

        }

        public UserApp RegisterNewUser(ApplicationContext applicationContext, string name, string lastname, string email, string password, DateTime localDate)
        {
            // DateTime localDate = DateTime.Now;


            try
            {

                applicationContext.Add(new UserApp()
                {
                    FirstName = name,
                    LastName = lastname,
                    Email = email,
                    Upassword = password,
                    IdRole = 1,
                    GroupStatus = false,
                    DateApp = localDate

                });

                applicationContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }

            try
            {
                return applicationContext.UserApps.Where(u => u.Email == email && u.Upassword == password)
                        .Select(u => u).FirstOrDefault();
            }

            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }

        }

        public bool CheckEmailExists(ApplicationContext applicationContext, string email)
        {
            bool response = false;
            UserApp userApp = null;
            try
            {
                userApp = applicationContext.UserApps.Where(u => u.Email == email)
                            .Select(u => u).FirstOrDefault();
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }

            if (userApp != null)
                response = true;

            return response;

        }


        public UserApp UpdatePassword(ApplicationContext applicationContext, string email, string password)
        {
            UserApp userApp = null;
            try
            {
                userApp = applicationContext.UserApps.Where(u => u.Email == email)
                             .Select(u => u).FirstOrDefault();
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }

            if (userApp != null)
            {
                try
                {
                    userApp.Upassword = password;
                    applicationContext.Update(userApp);
                    applicationContext.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    throw new Exception(e.Message);
                }
            }
            return userApp;
        }

        public UserApp GetUserById(ApplicationContext applicationContext, long? id)
        {
            try
            {

                var user = applicationContext.UserApps.Where(u => u.Id==id)
                            .Select(u => u).FirstOrDefault();
                return user;
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }
        }

        public UserApp GetUserByEmail(ApplicationContext applicationContext, long id, string email)
        {
            try
            {
                UserApp user;

                 user = applicationContext.UserApps.Where(u => u.Id == id && u.Email==email)
                            .Select(u => u).FirstOrDefault();
                return user;
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }

        }    
    }
}
