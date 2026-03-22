using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Models
{
    /// <summary>
    /// Представляет модель запроса, используемую для регистрации нового пользователя в системе.
    /// Все поля являются обязательными (<c>required</c>) и доступными только для инициализации (<c>init</c>).
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Обязательное имя пользователя (например, имя).
        /// </summary>
        public required string UserName { get; init; }

        public required string FirstName { get; init; }

        /// <summary>
        /// Обязательная фамилия пользователя.
        /// </summary>
        public required string LastName { get; init; }

        /// <summary>
        /// Обязательный адрес электронной почты пользователя. Используется для аутентификации.
        /// </summary>
        public required string Email { get; init; }

        /// <summary>
        /// Обязательный пароль для новой учетной записи. Должен быть хеширован сервисом идентификации.
        /// </summary>
        public required string Password { get; init; }
        public required string PhoneNumber { get; init; }
        public string? Role { get; init; }
    }
}
