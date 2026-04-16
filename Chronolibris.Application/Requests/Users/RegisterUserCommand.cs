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
    public record RegisterUserCommand(
        [RegularExpression("^(?=.*[a-zA-Z])[a-zA-Z0-9_]{5,32}$", ErrorMessage ="От 5 до 32 символов: латиница, цифры или _")]
        [MaxLength(32, ErrorMessage ="Имя пользователя должно быть не более 32 символов")]
        [MinLength(5, ErrorMessage ="Имя пользователя должно быть не менее 5 символов")]
        string UserName,
        [RegularExpression(@"^[\p{L}\s-]{1,64}$", ErrorMessage = "Имя содержит недопустимые символы")]
        [MaxLength(64, ErrorMessage ="Имя должно быть не менее 64 символов")]
        [MinLength(1, ErrorMessage = "Имя должно быть указано")]
        string FirstName, 
        [RegularExpression(@"^[\p{L}\s-]{1,64}$", ErrorMessage = "Фамилия содержит недопустимые символы")]
        [MaxLength(64, ErrorMessage = "Фамилия пользователя должна быть не более 64 символов")]
        [MinLength(1, ErrorMessage = "Фамилия должна быть указана")]
        string LastName, 
        [MaxLength(256, ErrorMessage = "Превышение допустимой длины")]
        [RegularExpression(@"^(?!.*\.\.)(?!^\.)(?!.*@\.)(?!.*@\-)[a-zA-Z0-9._%+-]+@(?!.*\-+\.)[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Введите почту в формате name@example.com")]
        [Required(ErrorMessage = "Адрес электронной почты обязателен")]
        string Email,
        [MaxLength(20, ErrorMessage = "Превышение допустимой длины")]
        [Required(ErrorMessage ="Номер телефона обязателен")]
        string PhoneNumber,
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,256}$", ErrorMessage ="Пароль должен быть длиной не менее 8 символов и содержать цифры," +
        " латинские заглавные и строчные буквы и один из символов #?!@$%^&*-")][MaxLength(256, ErrorMessage = "Превышение допустимой длины")]
        [Required(ErrorMessage ="Пароль обязателен")]
        string Password) : IRequest<RegistrationResult>;
}
