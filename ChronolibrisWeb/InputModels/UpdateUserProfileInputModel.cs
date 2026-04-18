using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public record UpdateUserProfileInputModel
    (   [RegularExpression("^(?=.*?[a-zA-Z])[a-zA-Z0-9_]{5,32}$", ErrorMessage ="От 5 до 32 символов: латиница, цифры или _")]
        [MaxLength(32, ErrorMessage = "Имя пользователя должно быть не более 32 символов")]
        [MinLength(5, ErrorMessage = "Имя пользователя должно быть не менее 5 символов")]
        string UserName,
        [RegularExpression(@"^(?=.*?\p{L})[\p{L}\s-]{1,64}$", ErrorMessage = "Имя содержит недопустимые символы")]
        [MaxLength(64, ErrorMessage = "Имя должно быть не менее 64 символов")]
        [MinLength(1, ErrorMessage = "Имя должно быть указано")]
        string FirstName,
        [RegularExpression(@"^(?=.*?\p{L})[\p{L}\s-]{1,64}$", ErrorMessage = "Фамилия содержит недопустимые символы")]
        [MaxLength(64, ErrorMessage = "Фамилия пользователя должна быть не более 64 символов")]
        [MinLength(1, ErrorMessage = "Фамилия должна быть указана")]
        string LastName,
        [MaxLength(256, ErrorMessage = "Превышение допустимой длины")]
        //[EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        [RegularExpression(@"^(?=^.{1,254}$)(?!.*\.\.)(?!^\.)(?!.*@\.)(?!.*@-)(?!.*\.@)[a-zA-Z0-9._%+-]+@(?!.*-\.)(?!.*\.-)[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Введите почту в формате name@example.com")]
        [Required(ErrorMessage = "Адрес электронной почты обязателен")]
        string Email,
        [RegularExpression(@"^(?:\+7|8)[0-9]{10}$", ErrorMessage ="Некорректный формат номера телефона")]
        [MaxLength(20, ErrorMessage = "Превышение допустимой длины")]
        [Required(ErrorMessage = "Номер телефона обязателен")]
        string PhoneNumber

    );
}
