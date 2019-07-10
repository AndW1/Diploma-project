using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.ViewModels.ViewModelTask
{
    public class UserTaskViewModel
    {
        public UserViewModel UserView { get; set; }
        public IEnumerable<BoardTask> BoardTasks { get; set; }
    }
}
