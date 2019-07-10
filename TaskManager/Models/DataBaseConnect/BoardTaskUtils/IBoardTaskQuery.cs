using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.DataBaseConnect.BoardTaskUtils
{
    public interface IBoardTaskQuery
    {
        IEnumerable<BoardTask> GetUsersBoardTask(ApplicationContext applicationContext, long userId);
        BoardTask GetBoardTask(ApplicationContext applicationContext, long taskId);
    }
}
