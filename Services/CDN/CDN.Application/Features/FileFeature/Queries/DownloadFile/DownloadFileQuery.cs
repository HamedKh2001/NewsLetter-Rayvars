using MediatR;

namespace CDN.Application.Features.FileFeature.Queries.DownloadFile
{
    public class DownloadFileQuery : IRequest<DownloadFileDto>
    {
        public long Id { get; set; }
        public int? Size { get; set; }

        public DownloadFileQuery(long id, int? size)
        {
            Id = id;
            Size = size;
        }
    }
}
