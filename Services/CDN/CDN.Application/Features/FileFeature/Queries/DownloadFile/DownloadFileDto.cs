using Newtonsoft.Json;
using System.IO;

namespace CDN.Application.Features.FileFeature.Queries.DownloadFile
{
    public class DownloadFileDto
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }

        [JsonIgnore]
        public MemoryStream Memory { get; set; }
    }
}
