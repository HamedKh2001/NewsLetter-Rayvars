using MediatR;
using Microsoft.AspNetCore.Http;

namespace CDN.Application.Features.FileFeature.Commands.UploadFile
{
    public class UploadFileCommand : IRequest<string>
    {
        public int CategoryId { get; set; }
        public IFormFile FormFile { get; set; }
        public string TagName { get; set; }
        public string FileName { get; set; }

        public UploadFileCommand(int categoryId, IFormFile formFile, string tagName, string fileName)
        {
            CategoryId = categoryId;
            FormFile = formFile;
            TagName = tagName;
            FileName = fileName;
        }
    }
}
