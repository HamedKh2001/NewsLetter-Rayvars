namespace CDN.Application.Common
{
    public static class FileExtensions
    {
        public static string GetStoredPath(this Domain.Entities.NewsLetter file)
        {
            string storedPath = Path.Combine(file.Category.Path, file.Category.Title);

            if (string.IsNullOrEmpty(file.TagName) == false)
                storedPath = Path.Combine(storedPath, file.TagName);

            return Path.Combine(storedPath, file.FileName);
        }
    }
}
