using System.ComponentModel.DataAnnotations;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public class RegisterStaffCommand : IRequest<RegistrationResult>
    {
        [MaxLength(256)]
        public required string UserName { get; set; }
        [MaxLength(256)]
        public required string LastName { get; set; }
        [MaxLength(256)]
        public required string FirstName { get; set; }
        [RegularExpression(@"^(?=^.{1,254}$)(?!.*\.\.)(?!^\.)(?!.*@\.)(?!.*@-)(?!.*\.@)[a-zA-Z0-9._%+-]+@(?!.*-\.)(?!.*\.-)[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Введите почту в формате name@example.com")]
        public required string Email { get; set; }
        [RegularExpression(@"^(?:\+7|8)[0-9]{10}$", ErrorMessage = "Некорректный формат телефона")]
        public required string PhoneNumber { get; set; }
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-+=/\\\\`:;{}()~[\\]\"'_<>|,.])[A-Za-z0-9#?!@$%^&*-+=/\\\\`:;{}()~[\\]\"'_<>|,.]{8,256}$",
        ErrorMessage = "Пароль должен быть длиной не менее 8 символов и содержать цифры," +
        " латинские заглавные и строчные буквы и один из символов #?!@$%^&*-")]
        public required string Password { get; set; }
        public required string Role { get; set; }
    }

}
