using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public record ChangePasswordCommand(string CurrentPassword, string NewPassword, long UserId) : IRequest<Unit>;
}
