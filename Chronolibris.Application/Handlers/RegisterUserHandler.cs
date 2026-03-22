using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Queries;
using Chronolibris.Application.Requests;
using MediatR;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.SystemConstants;
using Chronolibris.Domain.Interfaces;

namespace Chronolibris.Application.Handlers
{
    /// <summary>
    /// Обработчик команды для регистрации нового пользователя в системе.
    /// Реализует интерфейс <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// для обработки <see cref="RegisterUserCommand"/> и возврата <see cref="RegistrationResult"/>.
    /// </summary>
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegistrationResult>
    {
        /// <summary>
        /// Приватное поле только для чтения для доступа к сервису аутентификации и идентификации.
        /// </summary>
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="RegisterUserHandler"/>.
        /// </summary>
        /// <param name="identityService">Сервис, отвечающий за логику регистрации и управления пользователями.</param>
        public RegisterUserHandler(IIdentityService identityService,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Обрабатывает команду регистрации нового пользователя.
        /// </summary>
        /// <remarks>
        /// Данный обработчик выполняет маппинг данных из <see cref="RegisterUserCommand"/> в объект <see cref="RegisterRequest"/>,
        /// который затем передается внешнему сервису <see cref="IIdentityService"/> для выполнения основной бизнес-логики 
        /// (валидация данных, хеширование пароля, создание записи пользователя и, возможно, генерация токена).
        /// </remarks>
        /// <param name="request">Объект команды, содержащий данные нового пользователя (Email, Имя, Фамилия, Пароль).</param>
        /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию.
        /// Результат задачи — объект <see cref="RegistrationResult"/>, содержащий статус регистрации (успех/неудача) и, при необходимости, сообщения об ошибках.
        /// </returns>
        public async Task<RegistrationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.RegisterUserAsync(new RegisterRequest
            {
                Email = request.Email,
                LastName = request.LastName,
                UserName = request.UserName,
                Password = request.Password,
                FirstName = request.FirstName,
                PhoneNumber = request.PhoneNumber,
            });

            if (result.Success)
            {
                var newUserId = result.UserId;
                var utcNow = DateTime.UtcNow;

                var defaultBelovedShelf = new Shelf
                {
                    CreatedAt = utcNow,
                    Id = 0,
                    Name = "Избранное",
                    ShelfTypeId = ShelfTypes.FAVORITES_CODE,
                    UserId = newUserId,
                };
                var defaultReadShelf = new Shelf
                {
                    CreatedAt = utcNow,
                    Id = 0,
                    Name = "Прочитанные",
                    ShelfTypeId = ShelfTypes.READ_CODE,
                    UserId = newUserId,
                };

                await _unitOfWork.Shelves.AddAsync(defaultBelovedShelf, cancellationToken);
                await _unitOfWork.Shelves.AddAsync(defaultReadShelf, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }
}
