using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests
{
    /// <summary>
    /// Команда для регистрации нового пользователя в системе.
    /// <para>
    /// Этот класс используется для передачи всех необходимых данных 
    /// для создания новой учетной записи.
    /// Все поля являются обязательными (<c>required</c>) и доступными только для инициализации (<c>init</c>).
    /// </para>
    /// </summary>
    public class RegisterUserCommand : IRequest<RegistrationResult>
    {
        /// <summary>
        /// Обязательное имя пользователя.
        /// Свойство доступно только для инициализации (<c>init</c>).
        /// </summary>
        public required string UserName { get; init; }

        public required string FirstName { get; init; }

        /// <summary>
        /// Обязательная фамилия пользователя.
        /// Свойство доступно только для инициализации (<c>init</c>).
        /// </summary>
        public required string LastName { get; init; }

        /// <summary>
        /// Обязательный адрес электронной почты пользователя. Используется в качестве логина.
        /// Свойство доступно только для инициализации (<c>init</c>).
        /// </summary>
        public required string Email { get; init; }
        public required string PhoneNumber { get; init; }

        /// <summary>
        /// Обязательный пароль, который будет использоваться для аутентификации.
        /// Свойство доступно только для инициализации (<c>init</c>).
        /// </summary>
        public required string Password { get; init; }
    }
}
