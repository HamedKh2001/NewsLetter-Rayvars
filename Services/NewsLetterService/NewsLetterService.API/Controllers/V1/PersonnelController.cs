using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsLetterService.Application.Features.PersonnelFeature.Commands;
using NewsLetterService.Application.Features.PersonnelFeature.Queries.GetPersonnels;
using SharedKernel.Common;

namespace NewsLetterService.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PersonnelController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedList<PersonnelDto>>> Post([FromQuery] GetPersonnelsQuery query, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult<PersonnelDto>> Post([FromBody] CreatePersonnelCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }
    }
}
