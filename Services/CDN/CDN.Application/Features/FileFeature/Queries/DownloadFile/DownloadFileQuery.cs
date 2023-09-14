using MediatR;

namespace CDN.Application.Features.FileFeature.Queries.DownloadFile
{
    public class DownloadFileQuery : IRequest<DownloadFileDto>
    {
        public int Id { get; set; }

        public DownloadFileQuery(int id)
        {
            Id = id;
        }
    }
}
