using CDN.Application.Features.CategoryFeature.Commands.CreateCategory;
using CDN.Application.Features.CategoryFeature.Commands.UpdateCategory;
using CDN.Application.Features.CategoryFeature.Queries.GetCategories;
using CDN.Application.Features.CategoryFeature.Queries.GetCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common;

namespace CDN.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CategoryController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedList<CategoryDto>>> Get([FromQuery] GetCategoriesQuery query, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<CategoryDto>> Get([FromRoute] GetCategoryQuery query, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var Category = await Mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { employeeId = Category.Id }, Category);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update([FromRoute] long Id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            if (Id != command.Id)
                return BadRequest();

            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }

    }
}
