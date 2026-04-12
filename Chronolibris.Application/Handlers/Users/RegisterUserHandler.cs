using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Interfaces;
using MediatR;
using Chronolibris.Domain.Entities;
using Chronolibris.Application.Requests.Users;
using Chronolibris.Domain.Utils;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Exceptions;

namespace Chronolibris.Application.Handlers.Users
{

    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegistrationResult>
    {

        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;


        public RegisterUserHandler(IIdentityService identityService,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }


        public async Task<RegistrationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
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

                if (!result.Success)
                    throw new ChronolibrisException(result.Message
                        ?? "Ошибка регистрации", ErrorType.Validation);

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
                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
