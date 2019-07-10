using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.Admin
{
    public class ViewUserModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime DateRegister { get; set; }
        public long CountTask { get; set; }
        public string Image { get; set; }
    }
}
