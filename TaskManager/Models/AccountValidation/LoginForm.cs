using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.AccountValidation
{
    public class LoginForm
    {
        [Required(ErrorMessage = "Не указана почта")]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "Длина строки должна быть от 8 до 10 символов")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Только латинские буквы и цифры")]
        public string Password { get; set; }  
    }
}
