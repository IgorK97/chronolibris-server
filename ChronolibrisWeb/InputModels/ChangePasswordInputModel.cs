using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class ChangePasswordInputModel
    {
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-+=/\\\\`:;{}()~[\\]\"'_<>|,.])[A-Za-z0-9#?!@$%^&*-+=/\\\\`:;{}()~[\\]\"'_<>|,.]{8,256}$",
        ErrorMessage = "Пароль должен быть длиной не менее 8 символов и содержать цифры," +
        " латинские заглавные и строчные буквы и один из символов #?!@$%^&*-")]
        [MaxLength(256, ErrorMessage = "Превышение допустимой длины")]
        [Required(ErrorMessage = "Пароль обязателен")]
        public required string CurrentPassword { get; init; }
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-+=/\\\\`:;{}()~[\\]\"'_<>|,.])[A-Za-z0-9#?!@$%^&*-+=/\\\\`:;{}()~[\\]\"'_<>|,.]{8,256}$",
        ErrorMessage = "Пароль должен быть длиной не менее 8 символов и содержать цифры," +
        " латинские заглавные и строчные буквы и один из символов #?!@$%^&*-")]
        [MaxLength(256, ErrorMessage = "Превышение допустимой длины")]
        [Required(ErrorMessage = "Пароль обязателен")]
        public required string NewPassword { get; init; }
    }
}
