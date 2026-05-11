using Microsoft.AspNetCore.Identity;

namespace Chronolibris.Infrastructure.Services.IdentityService
{
    public class RussianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError { Code = nameof(DuplicateEmail), Description = $"Email '{email}' уже занят" };
        }

        public override IdentityError InvalidEmail(string? email)
        {
            return new IdentityError { Code = nameof(InvalidEmail), Description = $"Email '{email}' некорректный" };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError { Code = nameof(PasswordTooShort), Description = $"Пароль должен содержать минимум {length} символов" };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Пароль должен содержать цифру" };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Пароль должен содержать заглавную букву" };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError { Code = nameof(DuplicateUserName), Description = $"Пользователь '{userName}' уже существует" };
        }
        public override IdentityError PasswordMismatch()
        {
            return new IdentityError { Code = nameof(PasswordMismatch), Description = $"Пароли не совпадают" };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError { Code = nameof(PasswordRequiresLower), Description = $"Пароль должен содержать строчную букву" };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = $"Этот пароль уже используется" };
        }

        public override IdentityError InvalidUserName(string? userName)
        {
            return new IdentityError { Code = nameof(InvalidUserName), Description = $"Некорректное имя пользователя" };
        }

    }
}
