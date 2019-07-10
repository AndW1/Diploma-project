using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.ViewModels.ViewModelTask
{
    public class UserViewModel
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        //[Required]
        [BindNever]
        public string ImagePath { get; set; }

        [Required]
        //[BindNever]
        public DateTime DateRegister { get; set; }
    }
}
