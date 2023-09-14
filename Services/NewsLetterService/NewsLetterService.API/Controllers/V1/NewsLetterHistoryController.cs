using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsLetterService.Application.Features.NewsLetterHistoryFeature.Commands.CreateNewsLetterHistory;
using NewsLetterService.Application.Features.NewsLetterHistoryFeature.Queries;

namespace NewsLetterService.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class NewsLetterHistoryController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Unit>> Post([FromBody] CreateNewsLetterHistoryCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<NewsLetterHistoryDto>>> GetMyNewsLetters([FromQuery] GetMyNewsLettersQuery query, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }
    }
}
