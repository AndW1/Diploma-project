using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.ViewModels.ViewModelStage
{
    public class BoardViewModel
    {
       
        public long Id { get; set; }
       
        public string Name { get; set; }
      
        public string Content { get; set; }
       
        public DateTime DateCreated { get; set; }
       
        public DateTime DateFinished { get; set; }
    
    }
}
