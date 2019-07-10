using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.DataBaseConnect.GeneralDbActions
{
    public class GeneralActions : IGeneralQuery
    {
        public void AddNewObject(ApplicationContext applicationContex, object obj)
        {
            try
            {
                applicationContex.Add(obj);
                applicationContex.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateObject(ApplicationContext applicationContex, object obj)
        {
            try
            {
                applicationContex.Update(obj);
                applicationContex.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }
        }      
    }
}
