using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Queries;
using Chronolibris.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YamlDotNet.Core.Tokens;

namespace ChronolibrisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterUserCommand request)
        {
            var result = await _mediator.Send(request);
            if (!result.Success || string.IsNullOrEmpty(result.Token))
            {
                return Unauthorized(new { message = result.Message });
            }
            var token = result.Token;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("token", token, cookieOptions);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginUserCommand request)
        {
            var result = await _mediator.Send(request);
            if(!result.Success || string.IsNullOrEmpty(result.Token))
            {
                return Unauthorized(new { message = result.Message });
            }
            var token = result.Token;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("token", token, cookieOptions);
            return Ok();
        }

        //[HttpPost("refresh")]
        //public async Task<ActionResult> Refresh(string refreshToken)
        //{
        //    var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));
        //    return Ok(result);
        //}

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long parsedUserId))
                return Unauthorized();

            //long parsedUserId = long.Parse(userId);
            try
            {
                var result = await _mediator.Send(new GetUserProfileQuery(parsedUserId));
                return result !=null ? Ok(result): NotFound();

            }
            catch (Exception ex) {
                return StatusCode(500, "Ошибка при получении профиля");
            }
        }

        [Authorize]
        [HttpPost("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileRequest request)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            var command = new UpdateUserProfileCommand
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.UserName,
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request) 
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var command = new ChangePasswordCommand
            {
                UserId = userId,
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword,
            };
            await _mediator.Send(command);

            return Ok(new { success = true, message = "Password changed successfully" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {

            Response.Cookies.Delete("token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return Ok();
        }
    }

    public class UpdateUserProfileRequest
    {
        public required string FirstName { get; init; }
        public required string LastName {get;init;}
        public string? Email { get; init;}
        public string? PhoneNumber { get; init; }
        public required string UserName { get; init; }

    }

    public class ChangePasswordRequest
    {
        public required string CurrentPassword { get; init; }
        public required string NewPassword { get; init; }
    }
}

   
