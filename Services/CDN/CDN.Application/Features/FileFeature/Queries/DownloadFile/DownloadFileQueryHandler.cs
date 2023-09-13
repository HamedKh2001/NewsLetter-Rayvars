using CDN.Application.Common;
using CDN.Application.Common;
using CDN.Application.Contracts.Infrastructure;
using MediatR;
using SharedKernel.Exceptions;

namespace CDN.Application.Features.FileFeature.Queries.DownloadFile
{
    public class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, DownloadFileDto>
    {
        private readonly INewsLetterCacheService _fileCacheService;

        public DownloadFileQueryHandler(INewsLetterCacheService fileCacheService)
        {
            _fileCacheService = fileCacheService;
        }

        public async Task<DownloadFileDto> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
        {
            var file = _fileCacheService.Get(request.Id);
            if (file is null || file.Category.IsActive == false)
                throw new NotFoundException(nameof(Domain.Entities.NewsLetter), request.Id);

            var fileInfoModel = GetFilePath(request, file);

            if (File.Exists(fileInfoModel.FullOutputPath))
                return await FileToMemoryStream(fileInfoModel);
            else
                throw new ApiException("File does not exist.");
        }

        private FileInfoModel GetFilePath(DownloadFileQuery request, Domain.Entities.NewsLetter file)
        {
            return new FileInfoModel { FileName = file.FileName, FileType = file.FileType, FullOutputPath = file.GetStoredPath() };
        }

        private static async Task<DownloadFileDto> FileToMemoryStream(FileInfoModel fileInfoModel)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(fileInfoModel.FullOutputPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return new DownloadFileDto
            {
                ContentType = fileInfoModel.FileType,
                FileName = fileInfoModel.FileName,
                Memory = memory
            };
        }
    }
}
