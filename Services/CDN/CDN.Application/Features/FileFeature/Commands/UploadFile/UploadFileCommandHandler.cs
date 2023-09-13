using CDN.Application.Common;
using CDN.Application.Contracts.Infrastructure;
using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Infrastructure;
using SharedKernel.Exceptions;

namespace CDN.Application.Features.FileFeature.Commands.UploadFile
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, string>
    {
        private readonly INewsLetterRepository _newsLetterRepository;
        private readonly ICategoryCacheService _categoryCacheService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UploadFileCommandHandler> _logger;

        public UploadFileCommandHandler(INewsLetterRepository newsLetterRepository, ICategoryCacheService categoryCacheService, IDateTimeService dateTimeService, IUnitOfWork unitOfWork, ILogger<UploadFileCommandHandler> logger)
        {
            _newsLetterRepository = newsLetterRepository;
            _categoryCacheService = categoryCacheService;
            _dateTimeService = dateTimeService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var category = CheckCategory(request);
            string newsLetterPath = string.Empty;
            string directoryPath = string.Empty;
            try
            {
                _unitOfWork.BeginTransaction();

                var newsLetter = NewFileEntity(request, category);
                await _newsLetterRepository.CreateAsync(newsLetter);
                newsLetter.Category = category;

                newsLetterPath = newsLetter.GetStoredPath();
                directoryPath = Path.GetDirectoryName(newsLetterPath);
                CreateDirectory(directoryPath);
                await CopyFileToDirectory(request, newsLetterPath);

                _unitOfWork.Commit();
                return $"File/Download/{newsLetter.Id}";
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                DeleteFileAndDirectory(newsLetterPath, directoryPath);

                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        private Category CheckCategory(UploadFileCommand request)
        {
            var category = _categoryCacheService.Get(request.CategoryId);
            if (category is null)
                throw new NotFoundException("The {id} or {secretKey} is invalid");
            return category;
        }

        private static void DeleteFileAndDirectory(string newsLetterPath, string directoryPath)
        {
            if (File.Exists(newsLetterPath) == true)
                File.Delete(newsLetterPath);

            if (Directory.Exists(directoryPath) == true)
                Directory.Delete(directoryPath);
        }

        private static void CreateDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath) == false)
                Directory.CreateDirectory(directoryPath);
        }

        private static async Task CopyFileToDirectory(UploadFileCommand request, string newsLetterPath)
        {
            using (var bits = new FileStream(newsLetterPath, FileMode.Create))
            {
                await request.FormFile.CopyToAsync(bits);
            }
        }

        private NewsLetter NewFileEntity(UploadFileCommand request, Category category)
        {
            return new NewsLetter
            {
                CategoryId = category.Id,
                FileName = string.IsNullOrEmpty(request.FileName) ? request.FormFile.FileName : $"{request.FileName}{Path.GetExtension(request.FormFile.FileName)}",
                FileSize = request.FormFile.Length,
                FileType = request.FormFile.ContentType,
                TagName = string.IsNullOrEmpty(request.TagName) == false ? TagNameToPath(request) : null,
                CreatedDate = _dateTimeService.Now,
                IsDeleted = false
            };
        }

        private static string TagNameToPath(UploadFileCommand request)
        {
            string[] pathArray = request.TagName.Trim().Split('-').Select(s => s.Trim()).ToArray();
            return string.Join("\\", pathArray);
        }
    }
}
