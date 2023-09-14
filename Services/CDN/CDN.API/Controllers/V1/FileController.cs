using CDN.Application.Features.FileFeature.Commands.UploadFile;
using CDN.Application.Features.FileFeature.Queries.DownloadFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDN.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FileController : ApiControllerBase
    {
        [HttpPost("[action]/{CategoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Upload([FromRoute] int CategoryId, IFormFile formFile, [FromQuery] string TagName, [FromQuery] string FileName, CancellationToken cancellationToken)
        {
            var query = new UploadFileCommand(CategoryId, formFile, TagName, FileName);
            var url = await Mediator.Send(query, cancellationToken);
            return Ok(url);
        }

        [HttpGet("[action]/{Id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Download(CancellationToken cancellationToken, [FromRoute] int Id)
        {
            var query = new DownloadFileQuery(Id);
            var file = await Mediator.Send(query, cancellationToken);
            return File(file.Memory, file.ContentType, file.FileName, true);
        }
    }
}