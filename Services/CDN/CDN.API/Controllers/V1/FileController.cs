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
        public async Task<ActionResult<string>> Upload([FromRoute] long CategoryId, IFormFile formFile, [FromQuery] string TagName, [FromQuery] string FileName, CancellationToken cancellationToken)
        {
            var query = new UploadFileCommand(CategoryId, formFile, TagName, FileName);
            var url = await Mediator.Send(query, cancellationToken);
            return Ok(url);
        }

        [HttpGet("[action]/{Id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Download(CancellationToken cancellationToken, [FromRoute] long Id, [FromQuery] int? Size)
        {
            var query = new DownloadFileQuery(Id, Size);
            var file = await Mediator.Send(query, cancellationToken);
            return File(file.Memory, file.ContentType, file.FileName, true);
        }
    }
}