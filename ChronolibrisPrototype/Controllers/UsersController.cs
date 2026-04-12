using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Chronolibris.Application.Handlers.Users;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Application.Requests.Users;
using ChronolibrisWeb.InputModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YamlDotNet.Core.Tokens;

namespace ChronolibrisWeb.Controllers
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
                Expires = DateTime.UtcNow.AddDays(365)
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
                Expires = DateTime.UtcNow.AddDays(365)
            };

            Response.Cookies.Append("token", token, cookieOptions);
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long parsedUserId))
                return Unauthorized();

            //long parsedUserId = long.Parse(userId);

                var result = await _mediator.Send(new GetUserProfileQuery(parsedUserId));
                
                return result !=null ? Ok(result): NotFound();


        }

        [Authorize]
        //[HttpPost("profile")]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileInputModel request)
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
        public async Task<IActionResult> ChangePassword(ChangePasswordInputModel request) 
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

            return Ok();
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

        [Authorize(Roles = "admin")]
        [HttpPost("staff")]
        public async Task<IActionResult> RegisterStaff(
            [FromBody] RegisterStaffCommand request)
        {
            var result = await _mediator.Send(request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok();
        }
    }
}

   
