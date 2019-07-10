using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.DataBaseConnect.GeneralDbActions
{
    public interface IGeneralQuery
    {
         void  UpdateObject(ApplicationContext applicationContex, object obj);
         void  AddNewObject(ApplicationContext applicationContex, object obj);
    }
}
