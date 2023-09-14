using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSO.Application.Contracts.Infrastructure;
using SSO.Application.Features.AuthenticationFeature.Commands.ChangePassword;
using SSO.Application.Features.AuthenticationFeature.Commands.ChangeUsersPassword;
using SSO.Application.Features.AuthenticationFeature.Commands.UpdateUserGroup;
using SSO.Application.Features.AuthenticationFeature.Queries.Authenticate;
using SSO.Application.Features.AuthenticationFeature.Queries.LogoutUser;
using SSO.Application.Features.AuthenticationFeature.Queries.RefreshToken;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.WebAPI.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthenticationController : ApiControllerBase
    {
        private readonly IUserContextService _userContextService;

        public AuthenticationController(IUserContextService userContextService)
        {
            _userContextService = userContextService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<AuthenticateDto>> Login([FromBody] AuthenticateQuery query, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        [Authorize]
        [HttpPut("[action]")]
        public async Task<ActionResult<AuthenticateDto>> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            if (command.UserId != _userContextService.CurrentUser.Id)
                return BadRequest();

            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [Authorize]
        [HttpPut("[action]")]
        public async Task<ActionResult<AuthenticateDto>> ChangeUsersPassword([FromBody] ChangeUsersPasswordCommand command, CancellationToken cancellationToken)
        {
            if (command.AdminUserId != _userContextService.CurrentUser.Id)
                return BadRequest();

            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPut("[action]")]
        [Authorize(Roles = "SSO-Admin")]
        public async Task<ActionResult<AuthenticateDto>> UpdateUserGroup([FromBody] UpdateUserGroupCommand command, CancellationToken cancellationToken)
        {
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout([FromBody] LogoutUserQuery query, CancellationToken cancellationToken)
        {
            if (_userContextService.CurrentUser.Id != query.UserId)
                return BadRequest();

            await Mediator.Send(query, cancellationToken);
            return NoContent();
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenQuery query, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        [HttpGet("[action]")]
        [Authorize]
        public IActionResult CheckStatus()
        {
            return Ok(HttpContext.User.Identity.IsAuthenticated);
        }
    }
}
