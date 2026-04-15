using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Chronolibris.Infrastructure.Services.IdentityService
{
    public class RussianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email) =>
            new() { Code = nameof(DuplicateEmail), Description = $"Email '{email}' уже занят" };

        public override IdentityError InvalidEmail(string? email) =>
            new() { Code = nameof(InvalidEmail), Description = $"Email '{email}' некорректный" };

        public override IdentityError PasswordTooShort(int length) =>
            new() { Code = nameof(PasswordTooShort), Description = $"Пароль должен содержать минимум {length} символов" };

        public override IdentityError PasswordRequiresDigit() =>
            new() { Code = nameof(PasswordRequiresDigit), Description = "Пароль должен содержать цифру" };

        public override IdentityError PasswordRequiresUpper() =>
            new() { Code = nameof(PasswordRequiresUpper), Description = "Пароль должен содержать заглавную букву" };

        public override IdentityError DuplicateUserName(string userName) =>
            new() { Code = nameof(DuplicateUserName), Description = $"Пользователь '{userName}' уже существует" };
        public override IdentityError PasswordMismatch() =>
            new() { Code = nameof(PasswordMismatch), Description = $"Пароли не совпадают" };


        public override IdentityError PasswordRequiresLower() =>
            new() { Code = nameof(PasswordRequiresLower), Description = $"Пароль должен содержать строчную букву" };


        public override IdentityError UserAlreadyHasPassword()=>
            new() { Code = nameof(UserAlreadyHasPassword), Description = $"Этот пароль уже используется" };


        public override IdentityError InvalidUserName(string? userName)=>
            new() { Code = nameof(InvalidUserName), Description = $"Некорректное имя пользователя" };

    }
}
