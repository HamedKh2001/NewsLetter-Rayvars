using MediatR;

namespace CDN.Application.Features.FileFeature.Queries.DownloadFile
{
    public class DownloadNewsLetterQuery : IRequest<DownloadNewsLetterDto>
    {
        public int Id { get; set; }

        public DownloadNewsLetterQuery(int id)
        {
            Id = id;
        }
    }
}
