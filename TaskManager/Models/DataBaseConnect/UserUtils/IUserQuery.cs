using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.DataBaseConnect.UserUtils
{
    public interface IUserQuery
    {
        UserApp GetUser(ApplicationContext applicationContext, string email, string password);

        UserApp RegisterNewUser(ApplicationContext applicationContext, string name, string lastname, string email, string password, DateTime localDate);

        bool CheckEmailExists(ApplicationContext applicationContext, string email);

        UserApp UpdatePassword(ApplicationContext applicationContext, string email, string password);

        UserApp GetUserById(ApplicationContext applicationContext, long? id);

        UserApp GetUserByEmail(ApplicationContext applicationContext, long id, string email);   
    }
}
