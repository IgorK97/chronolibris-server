using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Queries;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    /// <summary>
    /// Обработчик команды для аутентификации пользователя (входа в систему).
    /// Реализует интерфейс <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// для обработки <see cref="LoginUserCommand"/> и возврата <see cref="LoginResult"/>.
    /// </summary>
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResult>
    {
        /// <summary>
        /// Приватное поле только для чтения для доступа к сервису аутентификации и авторизации.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LoginUserHandler"/>.
        /// </summary>
        /// <param name="identityService">Сервис, отвечающий за логику аутентификации (проверку учетных данных, генерацию токена).</param>
        public LoginUserHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Обрабатывает команду входа пользователя в систему.
        /// </summary>
        /// <remarks>
        /// Делегирует задачу аутентификации сервису <see cref="IIdentityService"/>, который проверяет 
        /// учетные данные пользователя (Email и Password) и, в случае успеха, 
        /// возвращает результат входа, обычно содержащий токен доступа. 
        /// </remarks>
        /// <param name="request">Объект команды, содержащий Email и Password пользователя.</param>
        /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию.
        /// Результат задачи — объект <see cref="LoginResult"/>, содержащий статус входа (успех/неудача) и, при успехе, необходимую информацию (например, JWT токен).
        /// </returns>
        public async Task<LoginResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.LoginUserByUserNameAsync(request.UserName, request.Password);
            return result;
        }
    }
}
