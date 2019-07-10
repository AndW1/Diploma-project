using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.DataBaseConnect.BoardTaskUtils
{
    public class BoardTaskUtils : IBoardTaskQuery
    {
        public BoardTask GetBoardTask(ApplicationContext applicationContext, long taskId)
        {
            try
            {
             return applicationContext.BoardTasks
                 .Include(b => b.IdOwnerNavigation)
                 .SingleOrDefault(t => t.Id == taskId);
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<BoardTask> GetUsersBoardTask(ApplicationContext applicationContext, long userId)
        {
            try
            {
                return applicationContext.BoardTasks.Include(d => d.IdOwnerNavigation).Where(t => t.IdOwner == userId).Select(t => t).ToList();  
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}
