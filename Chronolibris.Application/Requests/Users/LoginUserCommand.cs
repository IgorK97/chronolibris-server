using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public record LoginUserCommand(
        [RegularExpression("^(?=.*?[a-zA-Z])[a-zA-Z0-9_]{5,32}$", ErrorMessage ="От 5 до 32 символов: латиница, цифры или _")]
        [MaxLength(32, ErrorMessage ="Имя пользователя должно быть не более 32 символов")]
        [MinLength(5, ErrorMessage ="Имя пользователя должно быть не менее 5 символов")]
    string UserName,
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-+=/\\\\`:;{}()~[\\]\"'_<>|,.])[A-Za-z0-9#?!@$%^&*-+=/\\\\`:;{}()~[\\]\"'_<>|,.]{8,256}$", 
        ErrorMessage ="Пароль должен быть длиной не менее 8 символов и содержать цифры," +
        " латинские заглавные и строчные буквы и один из символов #?!@$%^&*-")]
        [MaxLength(256, ErrorMessage = "Превышение допустимой длины")]
        [Required(ErrorMessage ="Пароль обязателен")]
    string Password) : IRequest<LoginResult>;
}
