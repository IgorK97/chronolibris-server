using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public record RegisterUserCommand(
        [MaxLength(256)]
        string UserName,
        [MaxLength(256)]
        string FirstName, 
        [MaxLength(256)]
        string LastName, 
        [MaxLength(256)]
        string Email, 
        [MaxLength(20)]
        string PhoneNumber, 
        [MaxLength(128)]
        string Password) : IRequest<RegistrationResult>;
}
