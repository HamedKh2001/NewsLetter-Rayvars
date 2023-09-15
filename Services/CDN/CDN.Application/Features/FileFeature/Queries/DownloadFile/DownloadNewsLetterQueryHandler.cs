using CDN.Application.Common;
using CDN.Application.Contracts.Infrastructure;
using MediatR;
using SharedKernel.Exceptions;

namespace CDN.Application.Features.FileFeature.Queries.DownloadFile
{
    public class DownloadNewsLetterQueryHandler : IRequestHandler<DownloadNewsLetterQuery, DownloadNewsLetterDto>
    {
        private readonly INewsLetterCacheService _fileCacheService;

        public DownloadNewsLetterQueryHandler(INewsLetterCacheService fileCacheService)
        {
            _fileCacheService = fileCacheService;
        }

        public async Task<DownloadNewsLetterDto> Handle(DownloadNewsLetterQuery request, CancellationToken cancellationToken)
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

        private FileInfoModel GetFilePath(DownloadNewsLetterQuery request, Domain.Entities.NewsLetter file)
        {
            return new FileInfoModel { FileName = file.FileName, FileType = file.FileType, FullOutputPath = file.GetStoredPath() };
        }

        private static async Task<DownloadNewsLetterDto> FileToMemoryStream(FileInfoModel fileInfoModel)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(fileInfoModel.FullOutputPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return new DownloadNewsLetterDto
            {
                ContentType = fileInfoModel.FileType,
                FileName = fileInfoModel.FileName,
                Memory = memory
            };
        }
    }
}
