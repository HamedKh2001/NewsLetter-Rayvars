using System.IO;
using System.Linq;

namespace CDN.Application.Common
{
    public static class PathExtensions
    {
        public static bool HasInvalidFileNameChars(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var invalid = Path.GetInvalidFileNameChars();
            return input.ToCharArray().Any(i => invalid.Contains(i));
        }
    }
}
