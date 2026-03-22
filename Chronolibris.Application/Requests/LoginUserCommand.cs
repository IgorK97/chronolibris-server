using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Queries
{
    /// <summary>
    /// Команда для аутентификации пользователя в системе по электронной почте и паролю.
    /// <para>
    /// Этот класс является <c>record</c> с позиционными параметрами, 
    /// что обеспечивает неизменяемость (immutability) данных запроса.
    /// </para>
    /// </summary>
    /// <param name="UserName">Адрес электронной почты, используемый для входа.</param>
    /// <param name="Password">Пароль пользователя.</param>
    /// <returns>Возвращает объект <see cref="LoginResult"/>, содержащий статус входа 
    /// и, при успехе, токен аутентификации.</returns>
    public record LoginUserCommand(string UserName, string Password) : IRequest<LoginResult>;
}
